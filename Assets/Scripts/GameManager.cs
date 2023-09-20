using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    Character.PlayerCharacter _player;

    public Character.PlayerCharacter Player
    {
        get
        {
            if (_player == null)
            {
                _player = GameObject.FindWithTag("Player").GetComponent<Character.PlayerCharacter>();
            }

            return _player;
        }
    }

}