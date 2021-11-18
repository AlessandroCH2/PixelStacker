﻿using PixelStacker.Logic.IO.Config;
using PixelStacker.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelStacker.Logic.CanvasEditor.History
{
    public class EditHistory
    {
        public List<HistoryRecord> HistoryPast { get; set; } = new List<HistoryRecord>();
        public List<HistoryRecord> HistoryFuture { get; set; } = new List<HistoryRecord>();
        public bool IsUndoEnabled => HistoryPast.Count > 0;
        public bool IsRedoEnabled => HistoryFuture.Count > 0;

        private RenderedCanvas Canvas = null;

        public EditHistory(RenderedCanvas canvas)
        {
            this.Canvas = canvas;
        }

        public void Clear()
        {
            this.HistoryFuture.Clear();
            this.HistoryPast.Clear();
        }

        /// <summary>
        /// Applies the change to the underlying canvas data and then queues the change record to the buffer queue.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="isForward">If true, it means you are DOING it. False = UNDO</param>
        private List<RenderRecord> ApplyChange(HistoryRecord records, bool isForward)
        {
            this.Canvas.IsCustomized = true;
            List<RenderRecord> output = new List<RenderRecord>();
            foreach (var record in records.ChangeRecords)
            {
                var colorInt = isForward ? record.PaletteIDAfter : record.PaletteIDBefore;
                if (this.Canvas.MaterialPalette[colorInt] == null) return output;
                output.Add(new RenderRecord()
                {
                    ChangedPixels = record.ChangedPixels,
                    PaletteID = colorInt
                });

                foreach (var xy in record.ChangedPixels)
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    this.Canvas.CanvasData.SetDirectly(xy.X, xy.Y, colorInt);
#pragma warning restore CS0618 // Type or member is obsolete
                }
            }

            return output;
        }


        public void AddChange(HistoryRecord record)
        {
            this.HistoryFuture.Clear();
            ApplyChange(record, true);
            this.HistoryPast.Add(record);

            while (this.HistoryPast.Count > 0 && this.HistoryPast.Count > Constants.MAX_HISTORY_SIZE)
            {
                this.HistoryPast.RemoveAt(0);
            }
        }

        /// <summary>
        /// Will re-render as well.
        /// </summary>
        public List<RenderRecord> RedoChange()
        {
            // Check if we can
            if (!this.IsRedoEnabled) return new List<RenderRecord>();

            var record = this.HistoryFuture.Last();
            this.HistoryFuture.RemoveAt(this.HistoryFuture.Count - 1);

            var renderRecords = ApplyChange(record, true);
            this.HistoryPast.Add(record);
            return renderRecords;
        }

        public List<RenderRecord> UndoChange()
        {
            // Check if we can
            if (!this.IsUndoEnabled) return new List<RenderRecord>();

            var record = this.HistoryPast.Last();
            this.HistoryPast.RemoveAt(this.HistoryPast.Count - 1);

            var renderRecords = ApplyChange(record, false);
            this.HistoryFuture.Add(record);
            return renderRecords;
        }
    }
}
