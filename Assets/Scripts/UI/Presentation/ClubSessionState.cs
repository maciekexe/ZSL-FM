namespace ZSLFM.UI
{
    /// <summary>
    /// Tymczasowy stan sesji UI przenoszący wybrany lub utworzony w Menu Głównym klub do sceny Dashboardu.
    /// W zadaniu M-8/H-9 zostanie zastąpiony stanem z warstwy Core.
    /// </summary>
    public static class ClubSessionState
    {
        public static ClubSetupSelection ActiveSelection { get; private set; }
        public static bool HasActiveCareer => ActiveSelection != null;

        public static void SetActiveSelection(ClubSetupSelection selection)
        {
            ActiveSelection = selection;
        }

        public static void Clear()
        {
            ActiveSelection = null;
        }
    }
}
