extern alias legacy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.News
{
    using legacy::Gcpe.Hub.Data.Entity;

    public abstract class HubModel : IDisposable
    {
        protected readonly HubEntities db;

        public HubModel()
        {
            db = new HubEntities();
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public virtual void Save()
        {
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException uEx)
            {
                List<string> errors = new List<string>();

                //e.g.
                //Violation of UNIQUE KEY constraint 'UK_Document_ReleaseIdSortIndex'. Cannot insert duplicate key in object 'dbo.NewsDocument'. The duplicate key value is (52050dd4-470c-4a00-b664-aa04150d8884, 0).
                //The statement has been terminated.

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

        protected User User
        {
            get
            {
                //if (HttpContext.Current.User != null)
                {
                    if (HttpContext.Current.Session["UserId"] == null)
                    {
                        using (HubEntities db2 = new HubEntities())
                        {
                            Guid userId = HttpContext.Current.User.Identity.GetGuid();

                            string displayName = HttpContext.Current.User.Identity.GetDisplayName();

                            if (System.Diagnostics.Debugger.IsAttached && userId == Guid.Empty)
                                return db.Users.Find(Guid.Empty);

                            var user = db2.Users.Find(userId);
                            if (user == null)
                            {
                                string emailAddress = HttpContext.Current.User.Identity.GetEmailAddress();
                                //Match by email address if Guid is not found.
                                //TODO: Determine why Guids have changed after Windows Server 2012 migration.
                                user = db.Users.SingleOrDefault(e => e.EmailAddress == emailAddress);

                                if (user == null)
                                {
                                    user = new legacy.Gcpe.Hub.Data.Entity.User();
                                    user.Id = userId;
                                    user.EmailAddress = emailAddress;

                                    db2.Users.Add(user);
                                }
                            }

                            user.DisplayName = displayName;
                            user.IsActive = true;

                            db2.SaveChanges();

                            HttpContext.Current.Session["UserId"] = userId;
                        }
                    }
                }

                return db.Users.Find((Guid)HttpContext.Current.Session["UserId"]);
            }
        }

        public static string EncodeGuid(Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray()).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }
        public static Guid DecodeGuid(string value)
        {
            return new Guid(Convert.FromBase64String(value.Replace("-", "+").Replace("_", "/") + "=="));
        }
   }
}