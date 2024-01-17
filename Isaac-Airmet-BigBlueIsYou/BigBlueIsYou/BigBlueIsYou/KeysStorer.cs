namespace BigBlueIsYou
{
  public class KeysStorer
  {
    private KeysStorer()
    {

    }

    private static readonly KeysStorer keysStorer = new KeysStorer();
    private KeyBindingStorage keys = new KeyBindingStorage();
    public static KeysStorer getKeysStorer()
    {
      return keysStorer;
    }

    public void setKeys(KeyBindingStorage keys)
    {
      this.keys = keys;
    }

    public KeyBindingStorage getKeys()
    {
      return keys;
    }
  }
}
