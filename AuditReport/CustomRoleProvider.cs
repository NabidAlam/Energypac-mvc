using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;
using AuditReport.Models;

namespace AuditReport
{
    public class CustomRoleProvider : RoleProvider
    {
        private EPARSDbContext db = new EPARSDbContext();

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        //public override string[] GetRolesForUser(string username)
        //{
        //    throw new NotImplementedException();
        //}

      
        public override string[] GetRolesForUser(string username)
        {
            
            //IUserRoleRepository userRoleRep = new EFUserRoleRepository();

            //var userRoles = userRoleRep.UserRoles.Where(s=>s.SCM_USER.USER_NAME==username);

            var userRoles = db.UsersRole.Where(s => s.User.UserName  == username);

            if (userRoles!=null)
            {                
                string[] rollNames = userRoles.Select(x=>x.Role.RoleName).ToArray();
                return rollNames;
            }

            return null;

           

        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}