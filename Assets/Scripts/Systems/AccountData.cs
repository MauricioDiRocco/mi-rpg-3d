using System;
using System.Collections.Generic;

[Serializable]
public class AccountData
{
    public string username;
    public string passwordHash;
    public List<CharacterData> characters = new List<CharacterData>();
}

[Serializable]
public class CharacterData
{
    public string characterName;
    public string race       = "Guerrero";
    public int    level      = 1;
    public int    strength   = 12;
    public int    vitality   = 10;
    public int    intelligence = 5;
    public int    dexterity  = 8;
    public int    statPoints = 0;
}
