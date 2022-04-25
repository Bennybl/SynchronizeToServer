using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace SynchronizeToServer
{
    class RavenAPI
    {
        IDocumentStore m_Store;
        public RavenAPI(string i_ServerURL, string i_DatabaseName)
        {
            connectToServer(i_ServerURL, i_DatabaseName);
        }

        public void connectToServer(string i_ServerURL, string i_DatabaseName)
        {
            using (IDocumentStore store = new DocumentStore
            {
                Urls = new[]                        // URL to the Server,
    {                                   // or list of URLs 
                 i_ServerURL  // to all Cluster Servers (Nodes)
    },
                Database = i_DatabaseName,             // Default database that DocumentStore will interact with
                Conventions = { }                   // DocumentStore customizations
            })
            {
                store.Initialize();                 // Each DocumentStore needs to be initialized before use.
                                                    // This process establishes the connection with the Server
                                                    // and downloads various configurations
                                                       // e.g. cluster topology or client configuration
            }
        }

        public void updateFile(object i_ObjectToUpdate, string i_FileName)
        {
            DeleteFromDatabase(i_FileName);
            uplaodFile(i_ObjectToUpdate, i_FileName);
        }
        public void uplaodFile(object i_ObjectToUpload, string i_FileName)
        {
            using (var session = m_Store.OpenSession())
            {
                session.Store(i_ObjectToUpload);


                // send all pending operations to server, in this case only `Put` operation
                session.SaveChanges();
            }
        }
        public void DeleteFromDatabase(string i_FileName)
        {
            using (var session = m_Store.OpenSession())
            {
                session.Delete(i_FileName);
                session.SaveChanges();
            }
        }
    }
}
