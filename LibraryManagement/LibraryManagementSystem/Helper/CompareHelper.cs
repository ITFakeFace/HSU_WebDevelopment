namespace LibraryManagementSystem.Helper
{
    public class CompareHelper<T>
    {
        public static bool IncludeAll(List<T>? source, List<T> dest)
        {
            if (source == null)
            {
                return true;
            }
            foreach (var src in source)
            {
                if (!dest.Contains(src))
                {
                    return false;
                }
            }
            return true;
        }
    }
}