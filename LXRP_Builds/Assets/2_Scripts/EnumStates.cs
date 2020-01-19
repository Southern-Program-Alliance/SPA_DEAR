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
    HIT_BY_CAR,
    GAME_OVER
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
    RULES,
    DONUTS
}

public enum EMissionType
{
    GET_TO_STATION,
    FIND_CORRECT_RULES,
    COLLECT_HOTDOGS,
    COLLECT_DONUTS,
    SHOPPING,
    FIND_MISTAKES
}

public enum EScoreEvent
{
    GAME_START,
    ON_ROAD,
    AT_STATION,
    FOUND_PLAYER
}