using StaRTS.Main.Models.ValueObjects;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TaskViewState
	{
		public EpisodeTaskVO CurrentTaskInfo;

		public bool IsGrind;

		public int GrindCount;

		public int TaskIndex;

		public bool TaskComplete;

		public bool ResearchComplete;

		public bool IsTimeGated;

		public bool ShowTop;

		public bool ShowBottom;

		public bool Reset;

		public List<TaskViewMoment> Moments;

		public TaskViewState()
		{
			this.Moments = new List<TaskViewMoment>();
		}

		public TaskViewState Copy()
		{
			TaskViewState taskViewState = new TaskViewState();
			taskViewState.CurrentTaskInfo = this.CurrentTaskInfo;
			taskViewState.IsGrind = this.IsGrind;
			taskViewState.TaskIndex = this.TaskIndex;
			taskViewState.TaskComplete = this.TaskComplete;
			taskViewState.ResearchComplete = this.ResearchComplete;
			taskViewState.IsTimeGated = this.IsTimeGated;
			taskViewState.ShowTop = this.ShowTop;
			taskViewState.ShowBottom = this.ShowBottom;
			taskViewState.GrindCount = this.GrindCount;
			taskViewState.Reset = this.Reset;
			for (int i = 0; i < this.Moments.Count; i++)
			{
				taskViewState.Moments.Add(this.Moments[i]);
			}
			return taskViewState;
		}
	}
}
