using System.Text;
using System.IO;
using UnityEngine;

namespace Assets.Src.Tools
{
    internal static class UserDataPermanentStorage
    {
        private static string _guidCache;

        private static bool _isGuidActivated;

        public static bool PlayerGuidExistsAndActive()
        {
            Debugger.Log(Application.persistentDataPath + "/userdata");
            if (!File.Exists(Application.persistentDataPath + "/userdata"))
            {
                Debug.LogWarning("Id file does not exist");
                return false;
            }

            GetGuestGuid();

            return _isGuidActivated;
        }

        public static void SavePlayerGuid(string guid)
        {
            SavePlayerGuid(guid, false);
        }

        private static void SavePlayerGuid(string guid, bool active)
        {
            _guidCache = guid;
            _isGuidActivated = active;

            using (FileStream dataFile = File.Create(Application.persistentDataPath + "/userdata"))
            {
                using (var writer = new StreamWriter(dataFile, Encoding.ASCII))
                {
                    Debug.Log(string.Format("Saving secureId = {0}", guid));
                    writer.WriteLine(guid);
                    var numberToWrite = active ? "X" : "O";
                    Debug.Log(string.Format("SecureId activated? {0}", numberToWrite));
                    writer.WriteLine(numberToWrite);
                }
            }
        }

        public static string GetGuestGuid()
        {
            if (_guidCache != null && !_guidCache.Equals(""))
            {
                Debug.Log("Id cache contains something. Not reading from file");
                return _guidCache;
            }

            string guid;
            bool active;
            using (FileStream dataFile = File.OpenRead(Application.persistentDataPath + "/userdata"))
            {
                using (var reader = new StreamReader(dataFile, Encoding.ASCII))
                {
                    guid = reader.ReadLine();
                    var number = reader.ReadLine();
                    active = number == "X";

                    Debug.Log(string.Format("SecureId = {0}, activated = {1}", guid, number));
                }
            }

            _guidCache = guid;
            _isGuidActivated = active;

            return _guidCache;
        }

        public static void ClearGuid()
        {
            _guidCache = null;
            _isGuidActivated = false;
            File.Delete(Application.persistentDataPath + "/userdata");
        }

        public static void ActivateGuid()
        {
            Debug.Log("Activating secureId");
            SavePlayerGuid(_guidCache, true);
        }
    }

   
}
