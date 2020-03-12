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
        SSMR,
        PaymentFail,
        PaymentSuccess
    }

    public enum SSMRNotificationEnum
    {
        None = 0,
        RegistrationCompleted,
        RegistrationCancelled,
        TerminationCompleted,
        TerminationCancelled,
        OpenMeterReadingPeriod,
        NoSubmissionReminder,
        MissedSubmission
    }
}