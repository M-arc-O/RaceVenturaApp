namespace RaceVentura.RaceVenturaApiModels
{
    public enum ErrorCodes
    {
        Duplicate = 1,
        UserUnauthorized = 2,
        NotFound = 3,
        MaxIdsReached = 4,
        AnswerIncorrect = 5,
        CoordinatesIncorrect = 6,
        NotActiveStage = 7,
        RaceNotStarted = 8,
        RaceEnded = 9
    }
}
