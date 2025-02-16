﻿using System;

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Model.Input;

namespace LiveSplit.UI.Components;

public class ExitCounterComponent : IComponent
{
    public ExitCounterComponent()
    {
        VerticalHeight = 10;
        Settings = new ExitCounterComponentSettings();
        Cache = new GraphicsCache();
        ExitCounterLabel = new SimpleLabel();
    }

    public ExitCounterComponentSettings Settings { get; set; }

    public GraphicsCache Cache { get; set; }

    public float VerticalHeight { get; set; }

    public float MinimumHeight { get; set; }

    public float MinimumWidth => ExitCounterLabel.X + ExitCounterLabel.ActualWidth;

    public float HorizontalWidth { get; set; }

    public IDictionary<string, Action> ContextMenuControls => null;

    public float PaddingTop { get; set; }
    public float PaddingLeft => 7f;
    public float PaddingBottom { get; set; }
    public float PaddingRight => 7f;

    protected SimpleLabel ExitCounterLabel = new();

    protected Font CounterFont { get; set; }

    private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        // Set Background colour.
        if (Settings.BackgroundColor.A > 0
            || Settings.BackgroundGradient != GradientType.Plain
            && Settings.BackgroundColor2.A > 0)
        {
            var gradientBrush = new LinearGradientBrush(
                        new PointF(0, 0),
                        Settings.BackgroundGradient == GradientType.Horizontal
                        ? new PointF(width, 0)
                        : new PointF(0, height),
                        Settings.BackgroundColor,
                        Settings.BackgroundGradient == GradientType.Plain
                        ? Settings.BackgroundColor
                        : Settings.BackgroundColor2);

            g.FillRectangle(gradientBrush, 0, 0, width, height);
        }

        // Set Font.
        CounterFont = Settings.OverrideCounterFont ? Settings.CounterFont : state.LayoutSettings.TextFont;

        // Calculate Height from Font.
        var textHeight = g.MeasureString("A", CounterFont).Height;
        VerticalHeight = 1.2f * textHeight;
        MinimumHeight = MinimumHeight;

        PaddingTop = Math.Max(0, ((VerticalHeight - 0.75f * textHeight) / 2f));
        PaddingBottom = PaddingTop;

        HorizontalWidth = ExitCounterLabel.X + ExitCounterLabel.ActualWidth + 5;

        // Set Counter Label.
        ExitCounterLabel.HorizontalAlignment = mode == LayoutMode.Horizontal ? StringAlignment.Center : StringAlignment.Center;
        ExitCounterLabel.VerticalAlignment = StringAlignment.Center;
        ExitCounterLabel.X = 10;
        ExitCounterLabel.Y = 0;
        ExitCounterLabel.Width = (width - 10);
        ExitCounterLabel.Height = height;
        ExitCounterLabel.Font = CounterFont;
        ExitCounterLabel.Brush = new SolidBrush(Settings.OverrideTextColor ? Settings.CounterTextColor : state.LayoutSettings.TextColor);
        ExitCounterLabel.HasShadow = state.LayoutSettings.DropShadows;
        ExitCounterLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
        ExitCounterLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
        
        ExitCounterLabel.Draw(g);
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);
    }

    public string ComponentName => "Exit Counter";

    public Control GetSettingsControl(LayoutMode mode)
    {
        return Settings;
    }

    public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
    {
        return Settings.GetSettings(document);
    }

    public void SetSettings(System.Xml.XmlNode settings)
    {
        Settings.SetSettings(settings);
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        int completedExitCount = 0;

        if (Settings.AutoTotalCount)
        {
            completedExitCount = Math.Max(completedExitCount, state.CurrentSplitIndex);
            ExitCounterLabel.Text = Settings.ExitCounterText + " " + completedExitCount.ToString() + "/" + state.Run.Count;
        }
        else
        {
            try
            {
                completedExitCount = int.Parse(state.CurrentSplit.Name) - 1;
            }
            catch
            {
                try
                {
                    completedExitCount = Math.Max(completedExitCount, state.CurrentSplitIndex);
                }
                catch { }
            }
            ExitCounterLabel.Text = Settings.ExitCounterText + " " + completedExitCount.ToString() + "/" + Settings.TotalExitCount.ToString();
        }

        Cache.Restart();
        Cache["ExitCounterLabel"] = ExitCounterLabel.Text;

        if (invalidator != null && Cache.HasChanged)
        {
            invalidator.Invalidate(0, 0, width, height);
        }
    }

    public void Dispose()
    {

    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }

}
