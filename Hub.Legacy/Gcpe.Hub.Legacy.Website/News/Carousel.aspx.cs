extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using legacy::Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.News
{
    public partial class Carousel : Hub.News.Page
    {
        legacy::Gcpe.Hub.Data.Entity.Carousel carousel = null;
        public bool isLastCarousel;

        public const legacy::Gcpe.Hub.Data.Entity.Justify LeftJustify = legacy::Gcpe.Hub.Data.Entity.Justify.Left;
        public const legacy::Gcpe.Hub.Data.Entity.Justify RightJustify = legacy::Gcpe.Hub.Data.Entity.Justify.Right;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");

            Hub.News.Site site = Master;
            site.MenuText = "BC Gov News";
            site.AddAppItem("BC Gov Corporate Calendar", "~/Calendar");
            site.AddAppItem("News Release Management", "~/News/ReleaseManagement/Drafts");

            site.AddNavigationItem("Files", "~/News/FileManagement");
            site.AddNavigationItem("Carousel", "~/News/Carousel");
            site.AddNavigationItem("Emergency Pin", "~/News/EmergencySlideManagement");
            site.AddNavigationItem("Live Feed", "~/News/LiveFeedManagement");
            //site.AddNavigationItem("Project Granville", "~/News/ProjectGranvilleManagement");

            Guid id;
            if (Guid.TryParse((string)Request.QueryString["Id"], out id))
            {
                carousel = DbContext.Carousels.FirstOrDefault(c => c.Id == id);
                isLastCarousel = (carousel == null || !DbContext.Carousels.Any(c => c.PublishDateTime > carousel.PublishDateTime));
            }

            if (carousel == null)
            {
                carousel = DbContext.Carousels.OrderByDescending(c => c.PublishDateTime).FirstOrDefault();
                isLastCarousel = true;
            }

            ScriptManager.GetCurrent(this).RegisterPostBackControl(ButtonSave);
        }

        protected DateTimeOffset CarouselPublishDateTime
        {
            get { return carousel.PublishDateTime.Value; }
        }

        protected IEnumerable<CarouselSlide> SlidesItems
        {
            get { return carousel.CarouselSlides.OrderBy(e => e.SortIndex); }
        }

        protected void btnSave_Click(object sender, EventArgs ev)
        {
            try
            {
                carousel.PublishDateTime = DateTime.Parse(publishDateTimePicker.Text);
                List<CarouselSlide> slides = SlidesItems.ToList();
                int i = 0;
                foreach (RepeaterItem item in rptSlides.Items)
                {
                    CarouselSlide carouselSlide = slides.ElementAt(i++);
                    Slide slide = carouselSlide.Slide;
                    if (carouselSlide.SortIndex>=0)
                    {
                        string headline = ((TextBox)item.FindControl("txtHeadline")).Text;
                        string summary = ((TextBox)item.FindControl("txtSummary")).Text;
                        string actionUrl = ((TextBox)item.FindControl("txtActionUrl")).Text;
                        string facebookPostUrl = ((TextBox)item.FindControl("txtFacebookPostUrl")).Text;
                        sbyte sortIndex = SByte.Parse(((HiddenField)item.FindControl("sortIndex")).Value);
                        Justify justify = ((RadioButton)item.FindControl("RadioLeft")).Checked ? Justify.Left : Justify.Right;
                        HttpPostedFile imageFile = Request.Files[slide.Id.ToString()];
                        bool hasNewImage = imageFile.ContentLength != 0 &&
                            (imageFile.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || imageFile.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase));

                        bool slideEdited = slide.Headline != headline || slide.Summary != summary ||
                            slide.ActionUrl != actionUrl || slide.FacebookPostUrl != facebookPostUrl ||
                            slide.Justify != justify || hasNewImage;

                        if (slideEdited || carouselSlide.SortIndex != sortIndex)
                        {
                            if (slideEdited)
                            {
                                bool sharedSlide = DbContext.CarouselSlides.Count(s => s.SlideId == carouselSlide.SlideId) > 1;
                                if (sharedSlide)
                                {
                                    slide = NewSlide();
                                    if (!hasNewImage)
                                    {
                                        slide.Image = carouselSlide.Slide.Image;
                                    }
                                    DbContext.CarouselSlides.Remove(carouselSlide); // SlideId is part of the primary key of a CarouselSlide and cannot be changed
                                    carouselSlide = NewCarouselSlide(carousel.Id, slide.Id);
                                }
                                slide.Headline = headline;
                                slide.Summary = summary;
                                slide.ActionUrl = actionUrl;
                                slide.FacebookPostUrl = facebookPostUrl;
                                slide.Justify = justify;
                                if (hasNewImage)
                                {
                                    slide.Image = new byte[imageFile.ContentLength];
                                    imageFile.InputStream.Read(slide.Image, 0, imageFile.ContentLength);
                                }
                            }
                            carouselSlide.SortIndex = sortIndex;
                            slide.Timestamp = DateTimeOffset.Now;
                        }
                    }
                    
                }
                DbContext.SaveChanges();
                DataBind();      
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException uEx)
            {
                List<string> errors = new List<string>();

                string exceptionMessage = uEx.InnerException.InnerException.Message;
                errors.Add(exceptionMessage);

                throw new HubModelException(errors);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException evEx)
            {
                List<string> errors = new List<string>();

                foreach (var error in evEx.EntityValidationErrors)
                    errors.AddRange(error.ValidationErrors.Select(e => e.ErrorMessage));

                throw new HubModelException(errors);
            }
        }
        protected void btnPrevious_Click(object sender, EventArgs ev)
        {
            legacy::Gcpe.Hub.Data.Entity.Carousel previousCarousel = DbContext.Carousels.Where(c => c.PublishDateTime < carousel.PublishDateTime).OrderByDescending(c => c.PublishDateTime).FirstOrDefault();
            if (previousCarousel != null)
            {
                Redirect("~/News/Carousel.aspx?Id=" + previousCarousel.Id);
            }
        }

        protected void btnNext_Click(object sender, EventArgs ev)
        {
            legacy::Gcpe.Hub.Data.Entity.Carousel nextCarousel = DbContext.Carousels.Where(c => c.PublishDateTime > carousel.PublishDateTime).OrderBy(c => c.PublishDateTime).FirstOrDefault();
            if (nextCarousel == null)
            {
                nextCarousel = NewCarousel(DateTime.Now.AddDays(1));

                foreach (CarouselSlide carouselSlide in SlidesItems)
                {
                    if (carouselSlide.SortIndex>=0)
                    {
                        NewCarouselSlide(nextCarousel.Id, carouselSlide.SlideId).SortIndex = carouselSlide.SortIndex;
                    }
                }
                DbContext.SaveChanges();
                UnpinEmergencySlide();
            }

            Redirect("~/News/Carousel.aspx?Id=" + nextCarousel.Id);
        }

        legacy::Gcpe.Hub.Data.Entity.Carousel NewCarousel(DateTime publishDateTime)
        {
            return DbContext.Carousels.Add(new legacy::Gcpe.Hub.Data.Entity.Carousel
            {
                Id = Guid.NewGuid(),
                PublishDateTime = publishDateTime,
                Timestamp = DateTime.Now
            });
        }
        CarouselSlide NewCarouselSlide(Guid carouselId, Guid slideId)
        {
            return DbContext.CarouselSlides.Add(new CarouselSlide
            {
                CarouselId = carouselId,
                SlideId = slideId
            });
        }
        Slide NewSlide()
        {
            Slide slideToReuse = DbContext.Slides.FirstOrDefault(s => !s.CarouselSlides.Any());
            if (slideToReuse != null)
            {
                return slideToReuse;
            }
            var oldestCarousels = DbContext.Carousels.Where(c => c.PublishDateTime < DateTime.Now).OrderBy(c => c.PublishDateTime); // do not count scheduled ones
            if (oldestCarousels.Count() > 5)
            {
                var oldestCarousel = oldestCarousels.First();
                DbContext.CarouselSlides.RemoveRange(oldestCarousel.CarouselSlides);
                DbContext.Carousels.Remove(oldestCarousel);
                DbContext.SaveChanges();
                return NewSlide();
            }
            return DbContext.Slides.Add(new Slide { Id = Guid.NewGuid() });
        }

        private void UnpinEmergencySlide()
        {
            HubEntities hub = new HubEntities();
            hub.SetAppSetting("IsPinnedSlide", "false");
            hub.SetAppSetting("IsPinnedSecondarySlide", "false");
            hub.SaveChanges();
        }

        
        private bool GetFeedState(string appSetting)
        {
            HubEntities hub = new HubEntities();
            string enabled = hub.GetAppSetting(appSetting);
            return enabled.ToLower() == "true";
        }
        private string GetFeedValue(string appSetting)
        {
            HubEntities hub = new HubEntities();
            string value = hub.GetAppSetting(appSetting);
            return value;
        }

    }
}