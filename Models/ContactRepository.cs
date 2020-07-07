using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;

namespace ContactDetailsApp.Models
{
    public class ContactRepository
    {
        private CloudTable mytable = null;
        public ContactRepository()
        {

            string connstring = GetConnectionString().GetAwaiter().GetResult();
            // var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=contactstorage7269;AccountKey=cohTOKAIm3kPq34we8evKSVIER6X5Jj9Un7ADtdk/62SZYTzV0vI4h0hzLbxFlj0SdrLscGIzVvOVpR98kBxbw==;EndpointSuffix=core.windows.net");    
            var storageAccount = CloudStorageAccount.Parse(connstring);


            var cloudTableClient = storageAccount.CreateCloudTableClient();
            mytable = cloudTableClient.GetTableReference("Contacts");

        }

        public IEnumerable<Contact> GetAll()
        {
            var query = new TableQuery<Contact>();
            var entties = mytable.ExecuteQuery(query);
            return entties;
        }
        public void CreateOrUpdate(Contact myTableOperation)
        {
            var operation = TableOperation.InsertOrReplace(myTableOperation);
            mytable.Execute(operation);
        }


        public Contact Get(string partitionKey, string RowId)
        {
            var operation = TableOperation.Retrieve<Contact>(partitionKey, RowId);
            var result = mytable.Execute(operation);
            return result.Result as Contact;
        }

        private async Task<string> GetConnectionString()
        {
            //Microsoft.Azure.KeyVault.Models.SecretBundle secret = new Microsoft.Azure.KeyVault.Models.SecretBundle();
           
                AzureServiceTokenProvider azureServiceTokenProvider =
                  new AzureServiceTokenProvider();

                KeyVaultClient keyVaultClient =
                new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
              

                    var secret = await keyVaultClient
                   .GetSecretAsync("https://contactsolutionkeyvault.vault.azure.net/secrets/sqlconnectionstringvalue/401b805f73ae4f4abf9dfb72b14a74c0")
                           .ConfigureAwait(false);


                return secret.Value;
            
        }
    }
    } 