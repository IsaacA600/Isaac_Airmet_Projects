using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace BigBlueIsYou
{
    // Structure for saved high scores
    public class KeyBindingStorage
    {
        public KeyBindingStorage() { }

        public KeyBindingStorage(List<Keys> keyBindings)
        {
            this.keyBindings = keyBindings;
        }

        public List<Keys> keyBindings { get; set; }
        public bool isSavingError = false;
        public bool isLoadingError = false;
        public bool missingOnPC = false;
        public bool isLoaded = false;
    }
}
