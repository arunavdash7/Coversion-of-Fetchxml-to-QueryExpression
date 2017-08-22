//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Connection_to_Dynamics_crm
//{
//    class Program
//    {
//       public  static void Main(string[] args)
//        {
//            string sourceEnvironment = "Url=https://hpsalesbuild.crm.dynamics.com;Username=hpisalesadmin@GDCRMDomain.onmicrosoft.com; Password=crmDyamics!365;authtype=Office365;Timeout=20";//AzureKeyVaultConnector.GetSecretValue(sourceEnvironmentSecretName, string.Empty);

//            //// Getting Organization Service Proxy
//            var connection = Connection.GetServiceConfiguration(sourceEnvironment);

//            EntityReference opp = new EntityReference("opportunity", new Guid("A0A059F2-0576-E711-8100-5065F38A5BA1"));

//            Entity quote = new Entity(Quote.EntityName);
//            quote[Quote.OpportunityId] = opp;
//            quote.Id = new Guid("6F71859F-0676-E711-8100-5065F38A5BA1");

//            connection.Update(quote);

//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Query;

namespace Connection_to_Dynamics_crm
{
    class Program
    {
        static IOrganizationService _service;
        public static void Main(string[] args)
        {
            ConnectToMSCRM("your username", "your password", "your url ie cmr go to  settings->customization->developer resources->paste the svc url");
            Guid userid = ((WhoAmIResponse)_service.Execute(new WhoAmIRequest())).UserId;
            if (userid != Guid.Empty)
            {
                Console.WriteLine("Connection Established Successfully");
                //EntityReference opp = new EntityReference("opportunity", new Guid("E0C54430-BE76-E711-8100-5065F38A5BA1"));

                //Entity quote = new Entity("quote");
                //quote["opportunityid"] = opp;
                //quote.Id = new Guid("0E0DE943-BE76-E711-8100-5065F38A5BA1");
                //_service.Update(quote);

                //Entity opportunity = new Entity("opportunity");
                //opportunity.Id = new Guid("8B329FD5-F77C-E711-8101-5065F38ACB61");
                //opportunity["estimatedclosedate"] =new DateTime(2017,7,10);
                //_service.Update(opportunity);
                //Console.WriteLine("Opporunity estimatedclosedate is updated");
                string fetchfromPluginAssembly = @"<fetch mapping='logical' version='1.0' output-format='xml-platform' distinct='false'>
                            <entity name='pluginassembly'>
                            <attribute name='name' />
                            <attribute name='sourcetype' />
                            <attribute name='version' />
                            <attribute name='createdby' />
                            <attribute name='createdon' />
                            <attribute name='modifiedby' />
                            <attribute name='modifiedon' />
                            <attribute name='path' />
                            <attribute name='pluginassemblyid' />
                            </entity>
                            </fetch>";
                var retrievePluginAssembly = _service.RetrieveMultiple(new FetchExpression(fetchfromPluginAssembly));
                DisplayPluginAssemblyQueryResults(retrievePluginAssembly);
                Console.WriteLine("fetchxml complete");
                Console.WriteLine("conversion of fetchxml to queryexpression");
                // Convert the FetchXML into a query expression.
                var conversionRequest = new FetchXmlToQueryExpressionRequest
                {
                    FetchXml = fetchfromPluginAssembly
                };

                var conversionResponse =
                    (FetchXmlToQueryExpressionResponse)_service.Execute(conversionRequest);

                // Use the newly converted query expression to make a retrieve multiple
                // request to Microsoft Dynamics CRM.
                QueryExpression queryExpression = conversionResponse.Query;
                Console.WriteLine("QueryExpression"+ queryExpression);

                EntityCollection result = _service.RetrieveMultiple(queryExpression);

                // Display the results.
                Console.WriteLine("\nOutput for query after conversion to QueryExpression:");
                DisplayPluginAssemblyQueryResults(result);
                Console.ReadKey();
            }
        }

        private static void DisplayPluginAssemblyQueryResults(EntityCollection result)
        {
            Console.WriteLine("List all Plugin Assemblies.");
            Console.WriteLine("===========================================================================");
            foreach (var entity in result.Entities)
            {
                //var opportunity = entity.ToEntity<Opportunity>();
                //    Console.WriteLine("Opportunity ID: {0}", opportunity.Id);
                //    Console.WriteLine("Opportunity: {0}", opportunity.Name);
                //    var aliased = (AliasedValue)opportunity["contact2.fullname"];
                //    var contactName = (String)aliased.Value;
                //    Console.WriteLine("Associated contact: {0}", contactName);
                //}
                //Console.WriteLine("<End of Listing>");
                //Console.WriteLine();
                string pluginAssemblyname = entity.GetAttributeValue<string>("name");
                Console.WriteLine("Plugin Names"+ pluginAssemblyname);
            }
        }
        public static void ConnectToMSCRM(string UserName, string Password, string SoapOrgServiceUri)
        {
            try
            {
                ClientCredentials credentials = new ClientCredentials();
                credentials.UserName.UserName = UserName;
                credentials.UserName.Password = Password;
                Uri serviceUri = new Uri(SoapOrgServiceUri);
                OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
                proxy.EnableProxyTypes();
                _service = (IOrganizationService)proxy;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to CRM " + ex.Message);
                Console.ReadKey();
            }
        }
    }
}
