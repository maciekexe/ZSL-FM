using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Wspólna fabryka powtarzalnych elementów wizualnych UI (odznaki pozycji i umiejętności)
    /// do reużycia w tabelach kadry i rynku transferowego (M-3).
    /// </summary>
    public static class PlayerUiElementFactory
    {
        public static Label CreatePositionBadge(string position)
        {
            Label posBadge = new Label(position);
            posBadge.AddToClassList("player-pos-badge");
            return posBadge;
        }

        public static Label CreateSkillBadge(int overallSkill)
        {
            Label skillBadge = new Label(overallSkill.ToString());
            skillBadge.AddToClassList("player-skill-badge");
            skillBadge.AddToClassList(PlayerListQueryHelper.GetSkillTierCssClass(overallSkill));
            return skillBadge;
        }

        public static void UpdateHeaderButtonLabel(Button button, string baseLabel, bool isActive, bool ascending)
        {
            if (button == null) return;

            if (isActive)
            {
                button.text = $"{baseLabel} {(ascending ? "^" : "v")}";
                button.AddToClassList("col-header-btn--active");
            }
            else
            {
                button.text = $"{baseLabel} <>";
                button.RemoveFromClassList("col-header-btn--active");
            }
        }
    }
}
