using System;
using System.IO;
using Bioware.Erf;
using Bioware.Key;

namespace Bioware
{
    public class ResourceManager : Container
    {
        public ResourceManager(params string[] list)
        {
            Add(list);
        }

        public ResourceManager()
        {
        }

        public void Add(params string[] list)
        {
            foreach (var path in list)
            {
                Container c = null;
                if (Directory.Exists(path))
                {
                    c = new DirectoryContainer(path);
                }
                else
                {
                    if (File.Exists(path))
                    {
                        if (KeyFile.IsKey(path))
                        {
                            c = new KeyFile(path);
                        }
                        else if (ErfFile.IsErf(path))
                        {
                            c = new ErfFile(path);
                        }
                        else
                        {
                            throw new ApplicationException("Impossible d'ajouter le chemin spécifié : " + path);
                        }
                    }
                }
                if (c == null)
                {
                    throw new ApplicationException("Le conteneur de ressource n'est pas initialisé correctement.");
                }
                foreach (ContentObject co in c)
                {
                    Add(co);
                }
            }
        }
    }
}