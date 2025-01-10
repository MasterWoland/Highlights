namespace Aion.Highlights.Data
{
    /// <summary>
    /// MRA: use const and not static for the fields to work as expected
    /// Each string value must be unique
    /// </summary>
    public struct ID
    {
        public const string E_DATA_FILE_INFO = "DataFileInfoEvent";
        public const string E_PREPARE_NEW_DATA = "PrepareNewDataEvent";
        public const string E_DATA_PREPARED = "DataReadyForHighlightEvent";
        public const string E_START_HIGHLIGHT = "StartHighlightEvent";
        public const string E_RESUME_PLAY = "ResumePlayEvent";
        public const string E_CAPTURE = "CaptureEvent";
        public const string E_PREPARE_NEXT_FRAME = "PrepareNextFrameEvent";
        public const string E_OUT_OF_ENTRIES = "OutOfEntriesEvent";
        public const string E_POSITIONS_UPDATED = "PositionsUpdatedEvent";
        public const string E_NEW_TRACKING_DATA_ENTRY = "NewTrackingDataEntry";
        public const string E_FINISHED_RENDERING_UPDATED_FRAME = "FinishedRenderingUpdatedFrameEvent";
        public const string E_SCREENSHOT_CAPTURED = "ScreenshotCapturedEvent";
        public const string E_FINISHED_ALL_ENTRIES = "FinishedAllEntriesEvent";
        public const string E_CAM_CAPTURE_COMPLETE = "CameraCaptureCompleteEvent";
        public const string E_CAPTURE_COMPLETE = "CaptureCompleteEvent";
        public const string E_FROZEN_FRAME_CAPTURED = "FrozenFrameCaptured";
        public const string E_FROZEN_FRAME_CAPTURED_MAIN = "FrozenFrameCapturedMain";
        public const string E_HIGHLIGHT_MOMENT_CAPTURE_COMPLETE = "HighlightMomentCaptureCompleteEvent";
        public const string E_HIGHLIGHT_MOMENT_DETECTED = "HighlightMomentDetectedEvent";
        public const string E_START_HIGHLIGHT_MOMENT = "StartHighlightMomentEvent";
        public const string E_CAPTURE_FROZEN_MOMENT = "CaptureFrozenMoment";
    }
}