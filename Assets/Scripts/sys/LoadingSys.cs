namespace sys
{
    public class LoadingSys : SingleMono<AssetSys>
    {
        public enum LoadingType
        {
            small,middle,big
        }

        public static void Open(LoadingType t = LoadingType.middle)
        {
            
        }
    }
}