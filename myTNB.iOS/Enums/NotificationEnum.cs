namespace myTNB.Enums
{
    public enum BCRMNotificationEnum
    {
        None = 0,
        NewBill,
        BillDue,
        Dunning,
        Disconnection,
        Reconnection,
        Promotion,
        News,
        Maintenance,
        SSMR
    }

    public enum SSMRNotificationEnum
    {
        RegistrationCompleted = 0,
        RegistrationCancelled,
        TerminationCompleted,
        TerminationCancelled,
        OpenMeterReadingPeriod,
        NoSubmissionReminder,
        MissedSubmission
    }
}