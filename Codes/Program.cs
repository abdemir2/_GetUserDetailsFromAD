using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetUserInfoFromAD
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, "192.168.1.201"))   // ***** DOMAIN IP ADDRESS
                {
                    using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                    {
                        Console.WriteLine("Please enter username: ");
                        string _UserName = Console.ReadLine().ToString().ToLower().Trim();

                        var _Result = searcher.FindAll().Where(x => x.SamAccountName.Contains(_UserName));

                        if (_Result.Count() > 0)   // ***** IF RESULT IS NULL, DO NOT CONTINUE
                        {
                            foreach (var result in _Result)  
                            {
                                DirectoryEntry DirEntry = result.GetUnderlyingObject() as DirectoryEntry;
                                Console.WriteLine("First Name: " + DirEntry.Properties["givenName"].Value);
                                Console.WriteLine("Last Name : " + DirEntry.Properties["sn"].Value);
                                Console.WriteLine("User principal name: " + DirEntry.Properties["userPrincipalName"].Value);
                                Console.WriteLine("**************************************************");

                                Console.WriteLine("DETAILED INFORMATION");
                                Console.WriteLine("*********************");

                                var Auth = result as AuthenticablePrincipal;
                                if (Auth != null)
                                {
                                    Console.WriteLine("Name: " + Auth.Name);
                                    Console.WriteLine("Last Logon Time: " + Auth.LastLogon);
                                    Console.WriteLine();
                                }

                                Console.WriteLine("Last Logon: " + DirEntry.Properties["lastLogon"]);
                                foreach (string name in DirEntry.Properties.PropertyNames)
                                {
                                    Console.WriteLine("{0}: {1}", name, DirEntry.Properties[name].Value);
                                }

                                Console.WriteLine();
                                Console.ReadKey();
                            }   
                        }
                        else
                        {
                            Console.WriteLine("User does not exist in domain!");
                            Console.ReadKey();
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Domain does not exist or incorrect!");
                Console.ReadKey();
                return;
            }
        }
    }
}