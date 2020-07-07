using ContactDetailsApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Web;
using Azure.Storage.Blobs;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;

public class ContactsController : Controller
{
   
    public ContactsController()
    {

    }
    //Get the data from Azure table    
    public IList<Contact> GetAll()
    {
        var repositoty = new ContactRepository();
        var entities = repositoty.GetAll().ToList();
        var model = entities.Select(x => new Contact
        {

            ContactNo = x.ContactNo,
            FirstName = x.FirstName,
            LastName = x.LastName,
            CellNo = x.CellNo
        });
         return entities;
    }

    public IActionResult Index()
    {
        var repositoty = new ContactRepository();
        List<Contact> list = GetAll().ToList();
        return View(list);
    }


    public IActionResult ConfirmDelete(string group, string id)
    {
        var repositoty = new ContactRepository();
        var item = repositoty.Get(group, id);
        return View("Delete", new Contact
        {
            Group = item.PartitionKey,

            ContactNo = item.ContactNo,
            FirstName = item.FirstName,
            LastName = item.LastName,
            CellNo = item.CellNo
        });

    }

   
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Create(Contact contacttabel)
    {
        string connstring = GetConnectionString().GetAwaiter().GetResult();
        var repositoty = new ContactRepository();
        repositoty.CreateOrUpdate(new Contact
        {
            PartitionKey = "1",
            RowKey = Guid.NewGuid().ToString(),
            ContactNo = contacttabel.ContactNo,
            FirstName = contacttabel.FirstName,
            LastName = contacttabel.LastName,
            CellNo = contacttabel.CellNo,
            PhotoFile = contacttabel.PhotoFile

        }) ;
        BlobServiceClient image = new BlobServiceClient(connstring);
        BlobContainerClient containerClient = image.GetBlobContainerClient("photos");
        containerClient.UploadBlob(contacttabel.FirstName + Convert.ToString(contacttabel.CellNo), contacttabel.PhotoFile.OpenReadStream());
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Edit(Contact customerModel)
    {
        var repositoty = new ContactRepository();
        repositoty.CreateOrUpdate(new Contact
        {


        });
        return RedirectToAction("GetAll");
    }

            private async Task<string> GetConnectionString()
        {
           // Microsoft.Azure.KeyVault.Models.SecretBundle secret = new Microsoft.Azure.KeyVault.Models.SecretBundle();
           
                AzureServiceTokenProvider azureServiceTokenProvider =
                  new AzureServiceTokenProvider();

                KeyVaultClient keyVaultClient =
                new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
               

                    var secret = await keyVaultClient
                   .GetSecretAsync("https://contactsolutionkeyvault.vault.azure.net/secrets/sqlconnectionstringvalue/401b805f73ae4f4abf9dfb72b14a74c0")
                           .ConfigureAwait(false);


               
                return secret.Value;
            
            }

    //public async Task<string> UploadImageAsync(HttpPostedFileBase imageToUpload)
    //{
    //    string imageFullPath = null;
    //    if (imageToUpload == null || imageToUpload.ContentLength == 0)
    //    {
    //        return null;
    //    }
    //    try
    //    {
    //        CloudStorageAccount cloudStorageAccount = ConnectionString.GetConnectionString();
    //        CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
    //        CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("mycontainer");

    //        if (await cloudBlobContainer.CreateIfNotExistsAsync())
    //        {
    //            await cloudBlobContainer.SetPermissionsAsync(
    //                new BlobContainerPermissions
    //                {
    //                    PublicAccess = BlobContainerPublicAccessType.Blob
    //                }
    //                );
    //        }
    //        string imageName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(imageToUpload.FileName);

    //        CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
    //        cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;
    //        await cloudBlockBlob.UploadFromStreamAsync(imageToUpload.InputStream);

    //        imageFullPath = cloudBlockBlob.Uri.ToString();
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //    return imageFullPath;
    //}
    //public static class ConnectionString
    //{
    //    static string account = Microsoft.Azure.CloudConfigurationManager.GetSetting("contactstorage7269");
    //    static string key = Microsoft.Azure.CloudConfigurationManager.GetSetting("cohTOKAIm3kPq34we8evKSVIER6X5Jj9Un7ADtdk/62SZYTzV0vI4h0hzLbxFlj0SdrLscGIzVvOVpR98kBxbw==");
    //    public static CloudStorageAccount GetConnectionString()
    //    {
    //        string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName=contactstorage7269;AccountKey=cohTOKAIm3kPq34we8evKSVIER6X5Jj9Un7ADtdk/62SZYTzV0vI4h0hzLbxFlj0SdrLscGIzVvOVpR98kBxbw==;EndpointSuffix=core.windows.net");
    //        return CloudStorageAccount.Parse(connectionString);
    //    }
    //}
}
