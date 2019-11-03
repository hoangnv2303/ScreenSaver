    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScreenSaver.Model
{
    public class UserModel
    {
        public Employee employee { get; set; }
        public class Employee
        {
            public int id { get; set; }
            public string name { get; set; }
            public string display_name { get; set; }
            public string sAMAccountName { get; set; }
            public string employee_id { get; set; }
            public string work_email { get; set; }
            public string access_token { get; set; }
            public List<Object> department { get; set; }

            public string departmentName
            {
                get
                {
                    try
                    {
                        return department[1].ToString().Split('/')[0].Trim();
                    }
                    catch
                    {
                        return "";
                    }
                }
            }
        }
    }
}