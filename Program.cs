using System;
using Swarmy;
using System.Globalization;

namespace SwarmyNET 
{
    class SwarmyMain
{        static async Task Main(string[] args)
        {

            var textInfo = CultureInfo.InvariantCulture.TextInfo;

            SwarmyCore swarmyCore = new SwarmyCore();

            IdentityManager identityManager = new IdentityManager();

            swarmyCore.createGmailAccount();
            identityManager.getRecordsCount();

            identityManager.identityConstructor(40, InitializationCommand.StartOver);


        }
    }
}
