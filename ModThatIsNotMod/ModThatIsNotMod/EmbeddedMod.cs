namespace ModThatIsNotMod
{
    public abstract class EmbeddedMod
    {
        /// <summary>
        /// Runs when the assembly is loaded from the .melon file.
        /// </summary>
        public virtual void OnAssemblyLoaded() { }
        public virtual void OnSceneWasLoaded(int buildIndex, string sceneName) { }
        public virtual void OnSceneWasInitialized(int buildIndex, string sceneName) { }
        public virtual void OnUpdate() { }
        public virtual void OnLateUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnApplicationQuit() { }
    }
}
