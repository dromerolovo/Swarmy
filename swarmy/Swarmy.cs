using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Playwright;


namespace Swarmy
{   
        

        public class SwarmyCore
        
    {
        public void createGmailAccount(int numberOfAccounts = 0) {
            
            IdentityManager identityManager = new IdentityManager();
            var available = identityManager.getRecordsCount();
            
            if(numberOfAccounts > identityManager.getRecordsCount()) {

                throw new Exception("There is no available identities without gmail account");
            }

            


        }
        
    }

        

}
