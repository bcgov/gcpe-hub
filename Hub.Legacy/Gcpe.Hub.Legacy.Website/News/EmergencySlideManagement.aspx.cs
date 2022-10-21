extern alias legacy;
using System;
using legacy::Gcpe.Hub.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gcpe.Hub.News
{
    public partial class SlidePinningManagement : Hub.News.Page
    {
        public const legacy::Gcpe.Hub.Data.Entity.Justify LeftJustify = legacy::Gcpe.Hub.Data.Entity.Justify.Left;
        public const legacy::Gcpe.Hub.Data.Entity.Justify RightJustify = legacy::Gcpe.Hub.Data.Entity.Justify.Right;
        string appSettingIsPinned = "IsPinnedSlide";
        string appSettingPinnedId = "PinnedSlideId";
        string appSettingPinnedCarouselId = "PinnedCarouselId";
        string appSettingSecondaryIsPinned = "IsPinnedSecondarySlide";
        string appSettingSecondaryPinnedId = "SecondarySlideId";
        string appSettingSecondaryCarouselId = "SecondaryCarouselId";
        legacy::Gcpe.Hub.Data.Entity.Slide slide = null;
        legacy::Gcpe.Hub.Data.Entity.Slide secondary_slide = null;
        
        bool IsPinned = false;
        bool IsSecondaryPinned = false;
        Guid PinnedId = new Guid();
        Guid SecondaryPinnedId = new Guid();
        Guid PinnedCarouselId = new Guid();
        Guid SecondaryCarouselId = new Guid();

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
            IsPinned = GetFeedState(appSettingIsPinned);
            IsSecondaryPinned = GetFeedState(appSettingSecondaryIsPinned);
            Guid.TryParse(GetFeedValue(appSettingPinnedId), out PinnedId);
            Guid.TryParse(GetFeedValue(appSettingPinnedCarouselId), out PinnedCarouselId);
            Guid.TryParse(GetFeedValue(appSettingSecondaryPinnedId), out SecondaryPinnedId);
            Guid.TryParse(GetFeedValue(appSettingSecondaryCarouselId), out SecondaryCarouselId);
            if (DbContext.Slides.FirstOrDefault(a => a.Id == PinnedId) != null)
            {
                slide = DbContext.Slides.FirstOrDefault(a => a.Id == PinnedId);

            }
            else
            {
                slide = new Slide
                {
                    Headline = string.Empty,
                    Summary = string.Empty,
                    ActionUrl = string.Empty,
                    FacebookPostUrl = string.Empty,
                    Justify = LeftJustify,
                    Image = null
                };
            }
            if (DbContext.Slides.FirstOrDefault(a => a.Id == SecondaryPinnedId) != null)
            {
                secondary_slide = DbContext.Slides.FirstOrDefault(a => a.Id == SecondaryPinnedId);
            }
            else
            {
                secondary_slide = new Slide
                {
                    Headline = string.Empty,
                    Summary = string.Empty,
                    ActionUrl = string.Empty,
                    FacebookPostUrl = string.Empty,
                    Justify = LeftJustify,
                    Image = null
                };
            }

            if (!IsPostBack)
            {
                SetControlText(PinPrimaryButton, !GetFeedState(appSettingIsPinned));
                SetControlText(PinSecondaryButton, !GetFeedState(appSettingSecondaryIsPinned));
            }
            ScriptManager.GetCurrent(this).RegisterPostBackControl(PinPrimaryButton);
            ScriptManager.GetCurrent(this).RegisterPostBackControl(UpdatePrimaryButton);
            ScriptManager.GetCurrent(this).RegisterPostBackControl(PinSecondaryButton);
            ScriptManager.GetCurrent(this).RegisterPostBackControl(UpdateSecondaryButton);
        }
        // pin and unpin the primary and secondary slide
        protected void btnTogglePinnedSlide(object sender, EventArgs ev)
        {
            LinkButton btn = (LinkButton)sender;
            var id = btn.CommandArgument.ToString();

            // create new Carousel when pinning or unpinning
            var nextCarousel = NewCarousel(DateTime.Now.AddSeconds(30));
            var last_carousel = DbContext.Carousels.OrderByDescending(c => c.PublishDateTime).FirstOrDefault();
            var SlidesItems = last_carousel.CarouselSlides.OrderBy(s => s.SortIndex);
            
            if (btn.CommandName == "primayButton") {
                IsPinned ^= true;
                if (!IsPinned)
                {
                    SlidesItems = last_carousel.CarouselSlides.Where(s => s.SortIndex != -2).OrderBy(s => s.SortIndex);
                }
                foreach (CarouselSlide carouselSlide in SlidesItems)
                {
                    NewCarouselSlide(nextCarousel.Id, carouselSlide.SlideId).SortIndex = carouselSlide.SortIndex;
                }
                if (IsPinned)
                {
                    nextCarousel.CarouselSlides.Add(new CarouselSlide
                    {
                        CarouselId = nextCarousel.Id,
                        SlideId = slide.Id,
                        SortIndex = -2
                    });
                } 
                DbContext.SaveChanges();
                SetFeedState(IsPinned, appSettingIsPinned);
            } else if (btn.CommandName == "secondaryButton")
            {
                IsSecondaryPinned = !IsSecondaryPinned;
                if (!IsSecondaryPinned)
                {
                    SlidesItems = last_carousel.CarouselSlides.Where(s => s.SortIndex != -1).OrderBy(s => s.SortIndex);
                }
                foreach (CarouselSlide carouselSlide in SlidesItems)
                {
                    NewCarouselSlide(nextCarousel.Id, carouselSlide.SlideId).SortIndex = carouselSlide.SortIndex;
                }
                if (IsSecondaryPinned)
                {
                    nextCarousel.CarouselSlides.Add(new CarouselSlide
                    {
                        CarouselId = nextCarousel.Id,
                        SlideId = secondary_slide.Id,
                        SortIndex = -1
                    });
                }
                DbContext.SaveChanges();
                SetFeedState(IsSecondaryPinned, appSettingSecondaryIsPinned);
            }
            SetControlText(PinPrimaryButton, !GetFeedState(appSettingIsPinned));
            SetControlText(PinSecondaryButton, !GetFeedState(appSettingSecondaryIsPinned));
        }

        //update emergency slides
        protected void btnUpdateEmergencySlide(object sender, EventArgs ev)
        {
            LinkButton btn = (LinkButton)sender;
            var id = btn.CommandArgument.ToString();
            
            var container = Master.FindControl("formContentPlaceHolder");

            if (btn.CommandName == "primayButton") {
                var oldSlide = slide;
                string headline = ((TextBox)container.FindControl("txtHeadline")).Text;
                string summary = ((TextBox)container.FindControl("txtSummary")).Text;
                string actionUrl = ((TextBox)container.FindControl("txtActionUrl")).Text;
                string facebookPostUrl = ((TextBox)container.FindControl("txtFacebookPostUrl")).Text;
                Justify justify = ((RadioButton)container.FindControl("RadioLeft")).Checked ? Justify.Left : Justify.Right;
                HttpPostedFile imageFile = Request.Files[id];
                bool hasNewImage = imageFile.ContentLength != 0 &&
                    (imageFile.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || imageFile.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase));
                bool slideEdited = slide.Headline != headline || slide.Summary != summary ||
                            slide.ActionUrl != actionUrl || slide.FacebookPostUrl != facebookPostUrl ||
                            slide.Justify != justify || hasNewImage;
                if (slideEdited)
                {
                    slide = DbContext.Slides.Add(new Slide
                    {
                        Id = Guid.NewGuid(),
                        Headline = headline,
                        Summary = summary,
                        ActionUrl = actionUrl,
                        FacebookPostUrl = facebookPostUrl,
                        Justify = justify
                    });
                    if (hasNewImage)
                    {
                        slide.Image = new byte[imageFile.ContentLength];
                        imageFile.InputStream.Read(slide.Image, 0, imageFile.ContentLength);
                    }
                    else
                    {
                        slide.Image = oldSlide.Image;
                    }
                    slide.Timestamp = DateTimeOffset.Now;
                }
                if (IsPinned)
                {
                    var nextCarousel = NewCarousel(DateTime.Now.AddSeconds(30));
                    var last_carousel = DbContext.Carousels.OrderByDescending(c => c.PublishDateTime).FirstOrDefault();
                    var SlidesItems = last_carousel.CarouselSlides.Where(s=>s.SortIndex!=-2).OrderBy(s => s.SortIndex);
                    foreach (CarouselSlide carouselSlide in SlidesItems)
                    {
                        NewCarouselSlide(nextCarousel.Id, carouselSlide.SlideId).SortIndex = carouselSlide.SortIndex;
                    }
                    nextCarousel.CarouselSlides.Add(new CarouselSlide
                    {
                        CarouselId = nextCarousel.Id,
                        SlideId = slide.Id,
                        SortIndex = -2
                    });
                } else {

                }

                DbContext.SaveChanges();
                SetFeedValue("PinnedSlideId", slide.Id.ToString());
            } else if (btn.CommandName == "secondaryButton") {
                var oldSlide = secondary_slide;
                string headline = ((TextBox)container.FindControl("txtSecondHeadline")).Text;
                string summary = ((TextBox)container.FindControl("txtSecondSummary")).Text;
                string actionUrl = ((TextBox)container.FindControl("txtSecondActionUrl")).Text;
                string facebookPostUrl = ((TextBox)container.FindControl("txtSecondFacebookPostUrl")).Text;
                Justify justify = ((RadioButton)container.FindControl("RadioButton1")).Checked ? Justify.Left : Justify.Right;
                HttpPostedFile imageFile = Request.Files[id];
                bool hasNewImage = imageFile.ContentLength != 0 &&
                    (imageFile.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || imageFile.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase));
                bool slideEdited = secondary_slide.Headline != headline || secondary_slide.Summary != summary ||
                            secondary_slide.ActionUrl != actionUrl || secondary_slide.FacebookPostUrl != facebookPostUrl ||
                            secondary_slide.Justify != justify || hasNewImage;
                if (slideEdited)
                {
                    secondary_slide = DbContext.Slides.Add(new Slide
                    {
                        Id = Guid.NewGuid(),
                        Headline = headline,
                        Summary = summary,
                        ActionUrl = actionUrl,
                        FacebookPostUrl = facebookPostUrl,
                        Justify = justify
                    });
                    if (hasNewImage)
                    {
                        secondary_slide.Image = new byte[imageFile.ContentLength];
                        imageFile.InputStream.Read(secondary_slide.Image, 0, imageFile.ContentLength);
                    }
                    else
                    {
                        secondary_slide.Image = oldSlide.Image;
                    }
                    secondary_slide.Timestamp = DateTimeOffset.Now;
                }
                if (IsSecondaryPinned)
                {
                    var nextCarousel = NewCarousel(DateTime.Now.AddSeconds(30));
                    var last_carousel = DbContext.Carousels.OrderByDescending(c => c.PublishDateTime).FirstOrDefault();
                    var SlidesItems = last_carousel.CarouselSlides.Where(s => s.SortIndex != -1).OrderBy(s => s.SortIndex);
                    foreach (CarouselSlide carouselSlide in SlidesItems)
                    {
                        NewCarouselSlide(nextCarousel.Id, carouselSlide.SlideId).SortIndex = carouselSlide.SortIndex;
                    }
                    nextCarousel.CarouselSlides.Add(new CarouselSlide
                    {
                        CarouselId = nextCarousel.Id,
                        SlideId = secondary_slide.Id,
                        SortIndex = -1
                    });
                }
                DbContext.SaveChanges();
                SetFeedValue("SecondarySlideId", secondary_slide.Id.ToString());
            }
        }

        protected void btnSwap(object sender, EventArgs ev)
        {
            var tempSlide = secondary_slide;
            secondary_slide = slide;
            slide = tempSlide;
            var tempPinned = IsPinnedSecondarySlide;
            IsSecondaryPinned = IsPinned;
            IsPinned = tempPinned;
            SetFeedState(IsPinned, appSettingIsPinned);
            SetFeedState(IsSecondaryPinned, appSettingSecondaryIsPinned);
            SetFeedValue(appSettingPinnedId, slide.Id.ToString());
            SetFeedValue(appSettingSecondaryPinnedId, secondary_slide.Id.ToString());
        }
        protected Slide PinnedSlide
        {
            get { return slide; }
        }

        protected Slide SecondarySlide
        {
            get { return secondary_slide; }
        }

        protected bool IsPinnedSlide
        {
            get { return IsPinned; }
        }
        protected bool IsPinnedSecondarySlide
        {
            get { return IsSecondaryPinned; }
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

        private void SetControlText(LinkButton button, bool enabled)
        {
            //PinButton1.InnerHtml = enabled ? "Live Feed is Enabled" : "Live Feed is Disabled";
            button.Text = enabled ? "<i class=\"fa fa-thumb-tack fa-lg unpinned\" aria-hidden=\"true\"></i>" : "<i class=\"fa fa-thumb-tack fa-lg pinned\" aria-hidden=\"true\"></i>";
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
        private void SetFeedValue(string appSetting, string appValue)
        {
            HubEntities hub = new HubEntities();
            hub.SetAppSetting(appSetting, appValue);
            hub.SaveChanges();
        }

        private void SetFeedState(bool enabled, string appSetting)
        {
            HubEntities hub = new HubEntities();
            hub.SetAppSetting(appSetting, enabled ? "true" : "false");
            hub.SaveChanges();
        }

        private void SetPinnedSlideId(string carouselId, string slideId)
        {
            HubEntities hub = new HubEntities();
            hub.SetAppSetting("PinnedSlideId", slideId);
            hub.SetAppSetting("PinnedCarouselId", carouselId);
            hub.SetAppSetting("LastCarouselUpdate", DateTime.Now.ToString());
            hub.SaveChanges();
        }

    }
}