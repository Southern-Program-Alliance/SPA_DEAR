public enum EGameState
{
    BLANK,
    BEGIN,
    PLACED,
    LEVEL2_START,
    PLAYER_START,
    QUEST_START,
    QUEST_COMPLETE,
    PLAYER_COMPLETE,
    HIT_BY_CAR
}

public enum EARState
{
    BLANK,
    TUTORIAL,
    PLACEMENT,
    HELP,
    PLACED
}

public enum ESpawnSelection
{
    PEDESTRIANS,
    VEHICLES,
    PLAYERS,
    RULES
}

public enum EMissionType
{
    FIND_CORRECT_RULES,
    GET_TO_STATION,
    COLLECT_HOTDOGS,
    SHOPPING,
    FIND_MISTAKES
}

public enum EScoreEvent
{
    GAME_START,
    ON_ROAD,
    FOUND_PLAYER
}