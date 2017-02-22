//------------------------------------------------------------------------------
// <copyright file="MobTimerToolWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace MobTimer.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for MobTimerToolWindowControl.
    /// </summary>
    public partial class MobTimerToolWindowControl : UserControl, IDisposable
    {
        private DispatcherTimer _countdownUpdateTimer;

        private CountdownTimer _mobTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MobTimerToolWindowControl"/> class.
        /// </summary>
        public MobTimerToolWindowControl()
        {
            this.InitializeComponent();
            _mobTimer = new CountdownTimer(MobTimer_Elapsed);


            var memberList = MemberListStorage.GetMemberList();
            foreach (var member in memberList)
            {
                mobMemberList.Items.Add(member);
            }

            pauseButton.Visibility = Visibility.Hidden;

            InitializeDispatchTimer();
        }

        private void InitializeDispatchTimer()
        {
            _countdownUpdateTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Render,
                delegate
                {
                    DisplayCurrentCountdown();
                    progressBar.Value = _mobTimer.Progress;
                },
                Application.Current.Dispatcher);
        }

        private void DisplayCurrentCountdown()
        {
            var timeLeft = _mobTimer.TimeLeft;

            timerCountdownLabel.Content = string.Format("{0:00}:{1:00}", timeLeft.Minutes, timeLeft.Seconds);
        }

        private object GetGuider()
        {
            SetNextMember();
            var currentGuider = mobMemberList.Items.CurrentItem;
            SetPreviousMember();
            return currentGuider;
        }

        private int GetTimerValue()
        {
            var timerSettingText = GetTimerSettingText();
            int timerIntervalMinutes;
            if (!int.TryParse(timerSettingText, out timerIntervalMinutes))
            {
                timerIntervalMinutes = 11;
            }

            return timerIntervalMinutes;
        }

        private string GetTimerSettingText()
        {
            return timerSetting.Dispatcher.Invoke(() => timerSetting.Text);
        }

        private static TimeSpan FromMinutes(int timerIntervalMinutes)
        {

#if DEBUG
            return TimeSpan.FromSeconds(timerIntervalMinutes);
#endif
            return TimeSpan.FromMinutes(timerIntervalMinutes);
        }

        private void MobTimer_Elapsed()
        {
            SetNextMember();

            var nextUp = mobMemberList.Items.CurrentItem.ToString();

            Application.Current.Dispatcher.Invoke(() =>
            {

                var popup = new MobTimerPopupDialog(nextUp);
                var result = popup.ShowModal();

                if (result.HasValue && result.Value)
                {
                    StartTimer();
                }
                else
                {
                    ResetTimer();
                }
            });
        }

        private void StartTimer()
        {
            int timerIntervalMinutes = GetTimerValue();

            var interval = FromMinutes(timerIntervalMinutes);

            _mobTimer.StartTimer(interval);
            _countdownUpdateTimer.Start();

            var guider = GetGuider();

            ShowPauseButton();
            UpdatePauseButton();
            Application.Current.Dispatcher.Invoke(() =>
            {
                currentDriver.Content = mobMemberList.Items.CurrentItem;
                currentGuider.Content = guider;
            });
            mobMemberList.SelectedItem = mobMemberList.Items.CurrentItem;
        }

        private void ResetTimer()
        {
            _countdownUpdateTimer.Stop();
            HidePauseButton();
            Application.Current.Dispatcher.Invoke(() =>
            {
                DisplayCurrentCountdown();
            });
        }

        private void HidePauseButton()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                pauseButton.Visibility = Visibility.Hidden;
            });
        }
        private void ShowPauseButton()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                pauseButton.Visibility = Visibility.Visible;
            });
        }

        private void SetNextMember()
        {
            mobMemberList.Items.MoveCurrentToNext();
            if (mobMemberList.Items.IsCurrentAfterLast)
            {
                mobMemberList.Items.MoveCurrentToFirst();
            }
        }

        private void SetPreviousMember()
        {
            mobMemberList.Items.MoveCurrentToPrevious();
            if (mobMemberList.Items.IsCurrentBeforeFirst)
            {
                mobMemberList.Items.MoveCurrentToLast();
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (mobMemberList.Items.Count == 0)
            {
                return;
            }

            mobMemberList.Items.MoveCurrentToPosition(Math.Max(0, mobMemberList.SelectedIndex));
            StartTimer();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mobTimer.IsRunning)
            {
                _mobTimer.PauseTimer();
            }
            else
            {
                _mobTimer.ContinueTimer();
            }
            UpdatePauseButton();
        }

        private void UpdatePauseButton()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                pauseButton.Content = _mobTimer.IsRunning ? "Pause" : "Continue";
            });
        }

        private void addNewMemberButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewMember();
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            MoveItem(-1);
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            MoveItem(1);
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            mobMemberList.Items.Remove(mobMemberList.SelectedItem);
        }

        private void newMemberTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                AddNewMember();
            }
        }

        private void AddNewMember()
        {
            var newMemberName = newMemberTextBox.Text;
            if (string.IsNullOrWhiteSpace(newMemberName))
            {
                return;
            }
            mobMemberList.Items.Add(newMemberName);
            newMemberTextBox.Text = "";
            newMemberTextBox.Focus();
        }

        private void MoveItem(int direction)
        {
            // Checking selected item
            if (mobMemberList.SelectedItem == null || mobMemberList.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = mobMemberList.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= mobMemberList.Items.Count)
                return; // Index out of range - nothing to do

            object selected = mobMemberList.SelectedItem;

            // Removing removable element
            mobMemberList.Items.Remove(selected);
            // Insert it in new position
            mobMemberList.Items.Insert(newIndex, selected);
            // Restore selection
            mobMemberList.SelectedIndex = newIndex;
        }

        public void Dispose()
        {
            var memberList = new string[mobMemberList.Items.Count];
            mobMemberList.Items.CopyTo(memberList, 0);
            MemberListStorage.StoreMemberList(memberList.ToList());
        }
    }
}