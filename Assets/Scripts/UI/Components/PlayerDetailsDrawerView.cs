using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Komponent UI bocznego panelu profilu zawodnika (Drawer) z paskami 6 atrybutów.
    /// Odpowiada wyłącznie za prezentację wybranego obiektu PlayerProfileDto.
    /// </summary>
    public sealed class PlayerDetailsDrawerView
    {
        private readonly struct StatBarBinding
        {
            public readonly VisualElement Bar;
            public readonly Label ValueLabel;
            public readonly Func<PlayerProfileDto, int> ValueSelector;

            public StatBarBinding(VisualElement bar, Label valueLabel, Func<PlayerProfileDto, int> valueSelector)
            {
                Bar = bar;
                ValueLabel = valueLabel;
                ValueSelector = valueSelector;
            }
        }

        private readonly VisualElement _drawerRoot;
        private readonly Label _detailName;
        private readonly Label _detailClub;
        private readonly Label _detailPosBadge;
        private readonly Label _detailOverallBadge;
        private readonly Label _detailAge;
        private readonly Label _detailWage;
        private readonly Label _detailValue;
        private readonly StatBarBinding[] _statBindings;

        private VisualElement _selectedRowElement;

        public PlayerDetailsDrawerView(VisualElement root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            _drawerRoot = root.Q<VisualElement>("PlayerDetailsDrawer");
            _detailName = root.Q<Label>("DetailPlayerName");
            _detailClub = root.Q<Label>("DetailClubName");
            _detailPosBadge = root.Q<Label>("DetailPosBadge");
            _detailOverallBadge = root.Q<Label>("DetailOverallBadge");
            _detailAge = root.Q<Label>("DetailAge");
            _detailWage = root.Q<Label>("DetailWage");
            _detailValue = root.Q<Label>("DetailValue");

            _statBindings = new[]
            {
                new StatBarBinding(root.Q<VisualElement>("BarPac"), root.Q<Label>("ValPac"), p => p.Pace),
                new StatBarBinding(root.Q<VisualElement>("BarSho"), root.Q<Label>("ValSho"), p => p.Shooting),
                new StatBarBinding(root.Q<VisualElement>("BarPas"), root.Q<Label>("ValPas"), p => p.Passing),
                new StatBarBinding(root.Q<VisualElement>("BarDri"), root.Q<Label>("ValDri"), p => p.Dribbling),
                new StatBarBinding(root.Q<VisualElement>("BarDef"), root.Q<Label>("ValDef"), p => p.Defending),
                new StatBarBinding(root.Q<VisualElement>("BarPhy"), root.Q<Label>("ValPhy"), p => p.Physicality)
            };

            Button btnClose = root.Q<Button>("BtnCloseDetails");
            if (btnClose != null)
            {
                btnClose.clicked += () =>
                {
                    Hide();
                    DeselectCurrentRow();
                };
            }
        }

        public void Show(PlayerProfileDto player, VisualElement rowElement, ClubThemeDto theme)
        {
            if (_drawerRoot == null || player == null) return;

            HighlightSelectedRow(rowElement, theme);

            if (_detailName != null) _detailName.text = player.FullName;
            if (_detailClub != null) _detailClub.text = player.ClubName;
            if (_detailPosBadge != null) _detailPosBadge.text = player.Position;
            if (_detailAge != null) _detailAge.text = $"{player.Age} lat";
            if (_detailWage != null) _detailWage.text = $"{player.Wage:N0} PLN";
            if (_detailValue != null) _detailValue.text = $"{player.Price:N0} PLN";

            UpdateOverallBadge(player.OverallSkill);

            foreach (StatBarBinding binding in _statBindings)
            {
                SetStatBar(binding.Bar, binding.ValueLabel, binding.ValueSelector(player));
            }

            _drawerRoot.RemoveFromClassList("hidden");
        }

        public void Hide()
        {
            if (_drawerRoot != null) _drawerRoot.AddToClassList("hidden");
        }

        public void DeselectCurrentRow()
        {
            if (_selectedRowElement == null) return;

            _selectedRowElement.RemoveFromClassList("player-row--selected");
            _selectedRowElement.style.backgroundColor = StyleKeyword.Null;
            ClubThemeApplier.SetBorderColor(_selectedRowElement, StyleKeyword.Null);
            _selectedRowElement = null;
        }

        private void HighlightSelectedRow(VisualElement rowElement, ClubThemeDto theme)
        {
            DeselectCurrentRow();
            _selectedRowElement = rowElement;

            if (_selectedRowElement == null) return;

            _selectedRowElement.AddToClassList("player-row--selected");
            if (theme != null)
            {
                Color bg = theme.AccentColor;
                bg.a = 0.14f;
                _selectedRowElement.style.backgroundColor = bg;
                ClubThemeApplier.SetBorderColor(_selectedRowElement, theme.AccentColor);
            }
        }

        private void UpdateOverallBadge(int overallSkill)
        {
            if (_detailOverallBadge == null) return;

            _detailOverallBadge.text = overallSkill.ToString();
            _detailOverallBadge.RemoveFromClassList("skill-high");
            _detailOverallBadge.RemoveFromClassList("skill-mid");
            _detailOverallBadge.RemoveFromClassList("skill-low");
            _detailOverallBadge.AddToClassList(PlayerListQueryHelper.GetSkillTierCssClass(overallSkill));
        }

        private static void SetStatBar(VisualElement bar, Label valLabel, int statValue)
        {
            if (valLabel != null) valLabel.text = statValue.ToString();
            if (bar == null) return;

            bar.style.width = new Length(Mathf.Clamp(statValue, 5, 100), LengthUnit.Percent);
            if (statValue >= 80) bar.style.backgroundColor = new StyleColor(new Color32(16, 185, 129, 255));
            else if (statValue >= 65) bar.style.backgroundColor = new StyleColor(new Color32(245, 158, 11, 255));
            else bar.style.backgroundColor = new StyleColor(new Color32(100, 116, 139, 255));
        }
    }
}
