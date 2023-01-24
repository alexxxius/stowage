﻿using System.IO;
using System.Text;
using System.Threading.Tasks;
using Config.Net;
using Stowage.Impl.Microsoft;
using Xunit;

namespace Stowage.Test.Impl
{
   [Trait("Category", "Integration")]
   public class AzureBlobTest
   {
      private IAzureBlobFileStorage _storage;

      public AzureBlobTest()
      {
         ITestSettings settings = new ConfigurationBuilder<ITestSettings>()
            .UseIniFile("c:\\tmp\\integration-tests.ini")
            .Build();

         _storage = (IAzureBlobFileStorage)Files.Of.AzureBlobStorage(
            settings.AzureStorageAccount, settings.AzureStorageKey, settings.AzureContainerName);

         //_storage = (IAzureBlobFileStorage)Files.Of.AzureBlobStorageWithLocalEmulator(settings.AzureContainerName);

      }

      [Fact]
      public async Task OpenWrite_Append_LargerAndLarger()
      {
         string path = $"/{nameof(OpenWrite_Append_LargerAndLarger)}.txt";

         await _storage.Rm(path);

         // write first chunk
         using(Stream s = await _storage.OpenAppend(path))
         {
            byte[] line1 = Encoding.UTF8.GetBytes("one");
            await s.WriteAsync(line1, 0, line1.Length);
         }

         // validate
         string content = await _storage.ReadText(path);
         Assert.Equal("one", content);

         // write second chunk
         using(Stream s = await _storage.OpenAppend(path))
         {
            byte[] line1 = Encoding.UTF8.GetBytes("two");
            await s.WriteAsync(line1, 0, line1.Length);
         }

         // validate
         content = await _storage.ReadText(path);
         Assert.Equal("onetwo", content);
      }
   }
}
