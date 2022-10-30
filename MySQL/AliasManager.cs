using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MySQL {
    public class AliasManager {
        public static string path;
        public AliasManager() {
        }

        public void init(string inputpath) {
            path = inputpath;
            if (!File.Exists(path)) {
                string[] temppath = path.Split('\\');
                string dicPath = string.Empty;
                string filePath = temppath[temppath.Length - 1];
                for (int i = 0; i < temppath.Length - 1; i++) {
                    dicPath += temppath[i] + "\\";
                }
                Directory.CreateDirectory(dicPath);
                File.Create(dicPath + "\\" + filePath).Close();
            }
        }

        public string searchAlias(string input) {
            string data;
            string[] dataAll = File.ReadAllLines(path);
            for (int i = 0; i < dataAll.Length; i++) {
                if (dataAll[i].Split('^')[0] == input) {
                    return dataAll[i].Split('^')[1];
                }
            }
            return null;
        }

        public void setAlias(string command, string alias) {
            string[] dataAll = File.ReadAllLines(path);
            List<string> data = new List<string>();
            bool exists = false;
            for (int i = 0; i < dataAll.Length; i++) {
                if (dataAll[i].Split('^')[0] == command) {
                    data.Add(command + "^" + alias);
                    exists = true;
                } else {
                    data.Add(dataAll[i]);
                }
            }
            if (!exists) {
                data.Add(command + "^" + alias);
            }
            string[] writeData = new string[data.Count];
            for (int i = 0; i < data.Count; i++) {
                writeData[i] = data[i];
            }
            File.WriteAllText(path, string.Empty);
            File.WriteAllLines(path, writeData);
        }

        public void clearAlias() {
            File.WriteAllText(path, string.Empty);
        }

        public void removeAlias(string command) {
            string[] dataAll = File.ReadAllLines(path);
            List<string> data = new List<string>();
            for (int i = 0; i < dataAll.Length; i++) {
                if (dataAll[i].Split('^')[0] != command) {
                    data.Add(dataAll[i]);
                }
            }
            string[] writeData = new string[data.Count];
            for (int i = 0; i < data.Count; i++) {
                writeData[i] = data[i];
            }
            File.WriteAllText(path, string.Empty);
            File.WriteAllLines(path, writeData);
        }
    }
}
