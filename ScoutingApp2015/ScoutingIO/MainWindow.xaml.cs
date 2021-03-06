﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ScoutingIO.ViewModel;
using ScoutingIO.Views;

namespace ScoutingIO
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		public MatchView MatchV
		{
			get
			{
				return MatchTab.Content as MatchView;
			}
		}

		public EventView EventV
		{
			get
			{
				return EventTab.Content as EventView;
			}
		}

		public TeamsView TeamsV
		{
			get
			{
				return TeamsTab.Content as TeamsView;
			}
		}

		public UpdateView UpdateV
		{
			get
			{
				return UpdateViewHolder.Children[0] as UpdateView;
			}
		}

		public MainWindow()
		{
			ScoutingData.ScoutingJson.Initialize(false);

			InitializeComponent();
		}

		private void EventView_GoToMatches(object sender, RoutedEventArgs e)
		{
			MainTabControl.SelectedIndex = 1;
		}

		private void EventView_GoToTeams(object sender, RoutedEventArgs e)
		{
			MainTabControl.SelectedIndex = 2;
		}

		// Sort of a cheaty pipeline here
		private void EventView_SendMatchesData(object sender, EventArgs<EventViewModel> e)
		{
			UpdateMatchTabEnabled();

			MatchV.SendData(sender, e);
			UpdateV.SendData(sender, e);
			TeamsV.SendInitData();
		}

		private void UpdateMatchTabEnabled()
		{
			if (EventV.ViewModel.Event == null ||
				TeamsV.ViewModel.Teams == null)
			{
				MatchTab.IsEnabled = false;
			}
			else
			{
				MatchTab.IsEnabled = true;
			}
		}

		private void TeamsView_SendData(object sender, EventArgs<TeamsViewModel> e)
		{
			UpdateMatchTabEnabled();

			EventV.SendTeamsData(sender, e);
		}

		private void window_Loaded(object sender, RoutedEventArgs e)
		{
			TeamsV.SendInitData();
			EventV.SendInitData();

			UpdateMatchTabEnabled();
		}
	}
}
