using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace BigBlueIsYou
{
  public class FileManager
  {
    private bool loadingBindings = false;
    private bool saving = false;
    private bool loadingLevels = false;
    private KeyBindingStorage currentBindings = new KeyBindingStorage();
    public LevelStorer levelStorer = LevelStorer.getLevelStorer();
    public KeysStorer keysStorer = KeysStorer.getKeysStorer();

    public KeyBindingStorage getCurrentBindings()
    {
      return currentBindings;
    }

    public void startLoadingKeyBindings(string fileName, KeyBindingStorage bindings)
    {
      lock (this)
      {
        if (!this.loadingBindings)
        {
          this.loadingBindings = true;
          getKeyBindings(fileName, bindings);
        }
      }
    }

    private async void getKeyBindings(string fileName, KeyBindingStorage bindings)
    {
      await Task.Run(() =>
      {
        using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
        {
          try
          {
            if (storage.FileExists(fileName))
            {
              using (IsolatedStorageFileStream fs = storage.OpenFile(fileName, FileMode.Open))
              {
                if (fs != null)
                {
                  XmlSerializer mySerializer = new XmlSerializer(typeof(KeyBindingStorage));
                  bindings = (KeyBindingStorage)mySerializer.Deserialize(fs);
                }
              }
            }
            else
            {
              bindings.missingOnPC = true;
            }
          }
          catch (IsolatedStorageException)
          {
            bindings.isLoadingError = true;
            loadingBindings = false;
            currentBindings = bindings;
            keysStorer.setKeys(bindings);
          }
        }
        bindings.isLoadingError = false;
        bindings.isLoaded = true;
        loadingBindings = false;
        currentBindings = bindings;
        keysStorer.setKeys(bindings);
      });
    }

    public void startSavingKeyBindings(string fileName, KeyBindingStorage bindings)
    {
      lock (this)
      {
        if (!saving)
        {
          saving = true;
          saveKeyBindings(fileName, bindings);
        }
      }
    }

    private async void saveKeyBindings(string fileName, KeyBindingStorage bindings)
    {
      await Task.Run(() =>
      {
        using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
        {
          try
          {
            using (IsolatedStorageFileStream fs = storage.OpenFile(fileName, FileMode.Create))
            {
              if (fs != null)
              {
                XmlSerializer mySerializer = new XmlSerializer(typeof(KeyBindingStorage));
                mySerializer.Serialize(fs, bindings);
              }
            }
          }
          catch (IsolatedStorageException)
          {
            bindings.isSavingError = true;
            saving = false;
            currentBindings = bindings;
            keysStorer.setKeys(bindings);
          }
        }
        bindings.isSavingError = false;
        saving = false;
        currentBindings = bindings;
        keysStorer.setKeys(bindings);
      });
    }

    public void startLoadingLevels(string fileName)
    {
      lock (this)
      {
        if (!loadingLevels)
        {
          loadingLevels = true;
          loadLevels(fileName);
        }
      }
    }

    // Assumes valid file
    private void loadLevels(string fileName)
    {
      string[] lines = File.ReadAllLines(fileName);
      bool isStartOfLevel = true;
      bool isLevelSize = false;
      int currentRow = 1;
      bool isSecondGroup = false;
      LevelDataContainer curLevel = new LevelDataContainer();
      for (int i = 0; i < lines.Length; i++)
      {
        if (isStartOfLevel)
        {
          curLevel.Name = lines[i];
          isStartOfLevel = false;
          isLevelSize = true;
        }
        else if (isLevelSize)
        {
          string[] dimensions = lines[i].Split(" x ");
          curLevel.width = System.Int32.Parse(dimensions[0]);
          curLevel.height = System.Int32.Parse(dimensions[1]);
          curLevel.data = new List<char>[curLevel.height, curLevel.width];
          for (int a = 0; a < curLevel.height; a++)
          {
            for (int b = 0; b < curLevel.width; b++)
            {
              curLevel.data[a, b] = new List<char>();
            }
          }
          isLevelSize = false;
          currentRow = 1;
        }
        else
        {
          char[] characters = lines[i].ToCharArray();
          for (int j = 0; j < curLevel.width; j++)
          {
            curLevel.data[currentRow - 1, j].Add(characters[j]);
          }
          currentRow++;
          if (currentRow > curLevel.height && isSecondGroup)
          {
            currentRow = 1;
            isStartOfLevel = true;
            levelStorer.addLevel(curLevel);
            curLevel = new LevelDataContainer();
            isSecondGroup = false;
          }
          else if (currentRow > curLevel.height)
          {
            isSecondGroup = true;
            currentRow = 1;
          }
        }
      }
    }
  }
}
