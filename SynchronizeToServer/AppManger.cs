using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace SynchronizeToServer
{
    class AppManger
    {
     
        private MetaData m_MetaData;
        private readonly Dictionary<string, bool> r_IsFileVisted = new Dictionary<string, bool>();
        private readonly RavenAPI ravenAPI;
        private const string C_PathToMetaData = @"C:\Users\User\source\repos\SynchronizeToServer\SynchronizeToServer\FilesInLastRun.Json";

        public AppManger(string i_PathToFolder, string i_PathToServer, string i_DatabaseName)
        {
            ravenAPI = new RavenAPI(i_PathToServer, i_DatabaseName);
            deserializeJsonToSet();
            createIsFileVistesDictionry();
            cheackForUpdate(i_PathToFolder);
            checkForDeletedFiles();
            serializeMetadataToJson();
        }
        /// <summary>
        /// deserialize Json files that store the name of the files we had in the last invocation into hashtable, and the time of the last invocation
        /// </summary>
        private void deserializeJsonToSet()
        {
            string filesNamesInJsonAsString = File.ReadAllText(C_PathToMetaData);
            m_MetaData = JsonSerializer.Deserialize<MetaData>(filesNamesInJsonAsString);
            
        }
        /// <summary>
        /// serialize the hashtabel with the files we had in the current invocation and the current time, into json file for the next invocation 
        /// </summary>
        private void serializeMetadataToJson()
        {
            m_MetaData.LastInvocationTime = DateTime.Now;
            string json = JsonSerializer.Serialize(m_MetaData);
            File.WriteAllText(C_PathToMetaData, json);
        }
        /// <summary>
        /// creates dictonry of files, key: name of the file, value: boolean flag indicating if the file was visted,
        /// if the file wan not visted, it had been removed from the floder, and we'll need to delete it from the server.
        /// </summary>
        private void createIsFileVistesDictionry()
        {
            foreach(string fileName in m_MetaData.FilesInFolder)
            {
                r_IsFileVisted.Add(fileName, false);
            }
        }

        private void cheackForUpdate(string i_PathToFolder)
        {
            var d = new DirectoryInfo(i_PathToFolder);
            foreach (FileInfo fi in d.GetFiles())
            {
                if (m_MetaData.FilesInFolder.Contains(fi.Name))
                {
                    r_IsFileVisted[fi.Name] = true;
                    if (!(fi.LastWriteTime < m_MetaData.LastInvocationTime))
                    {
                        
                        var jsonFile = JsonSerializer.Deserialize<object>(File.ReadAllText(fi.FullName));
                        ravenAPI.updateFile(jsonFile , fi.Name);
                    }
                }
                else
                {
                    r_IsFileVisted.Add(fi.Name, true);
                    m_MetaData.FilesInFolder.Add(fi.Name);
                    var jsonFile = JsonSerializer.Deserialize<object>(File.ReadAllText(fi.FullName));
                    ravenAPI.updateFile(jsonFile, fi.Name);
                }
            }
                
        }
        /// <summary>
        /// Iterates over the isVisted dictionry, and deletes from the server, the files with false value.
        /// </summary>
        private void checkForDeletedFiles()
        {
            foreach(string fileName in r_IsFileVisted.Keys)
            {
                if (!r_IsFileVisted[fileName])
                {
                    ravenAPI.DeleteFromDatabase(fileName);
                    m_MetaData.FilesInFolder.Remove(fileName);
                }
            }
        }


    }
}
