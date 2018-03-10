﻿using System;
using System.Windows.Forms;
using ResourceManager;

namespace GitUI.CommandsDialogs.SettingsDialog
{
    public interface IGlobalSettingsPage : ISettingsPage
    {
        void SetGlobalSettings();
    }

    public interface ILocalSettingsPage : IGlobalSettingsPage
    {
        void SetLocalSettings();

        void SetEffectiveSettings();
    }

    public interface IRepoDistSettingsPage : ILocalSettingsPage
    {
        void SetRepoDistSettings();
    }

    public partial class SettingsPageHeader : GitExtensionsControl
    {
        private readonly SettingsPageWithHeader _page;

        public SettingsPageHeader(SettingsPageWithHeader page)
        {
            InitializeComponent();
            Translate();

            if (page != null)
            {
                settingsPagePanel.Controls.Add(page);
                page.Dock = DockStyle.Fill;
                _page = page;
                ConfigureHeader();
            }
        }

        private void ConfigureHeader()
        {
            ILocalSettingsPage localSettings = _page as ILocalSettingsPage;

            if (localSettings == null)
            {
                GlobalRB.Checked = true;

                EffectiveRB.Visible = false;
                DistributedRB.Visible = false;
                LocalRB.Visible = false;
                arrows1.Visible = false;
                arrows2.Visible = false;
                arrow3.Visible = false;
                tableLayoutPanel2.RowStyles[2].Height = 0;
            }
            else
            {
                LocalRB.CheckedChanged += (a, b) =>
                {
                    if (LocalRB.Checked)
                    {
                        localSettings.SetLocalSettings();
                    }
                };

                EffectiveRB.CheckedChanged += (a, b) =>
                {
                    if (EffectiveRB.Checked)
                    {
                        arrows1.ForeColor = EffectiveRB.ForeColor;
                        localSettings.SetEffectiveSettings();
                    }
                    else
                    {
                        arrows1.ForeColor = arrows1.BackColor;
                    }

                    arrows2.ForeColor = arrows1.ForeColor;
                    arrow3.ForeColor = arrows1.ForeColor;
                };

                EffectiveRB.Checked = true;

                IRepoDistSettingsPage repoDistPage = localSettings as IRepoDistSettingsPage;

                if (repoDistPage == null)
                {
                    DistributedRB.Visible = false;
                    arrow3.Visible = false;
                }
                else
                {
                    DistributedRB.CheckedChanged += (a, b) =>
                    {
                        if (DistributedRB.Checked)
                        {
                            repoDistPage.SetRepoDistSettings();
                        }
                    };
                }
            }
        }

        private void GlobalRB_CheckedChanged(object sender, EventArgs e)
        {
            if (GlobalRB.Checked)
            {
                _page.SetGlobalSettings();
            }
        }
    }
}
