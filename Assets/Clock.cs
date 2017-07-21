using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Diagnostics;
using System;

public static class Clock
{
    // amount of ms the turn on the main board takes (turn= both players played)
    //the fraction of the largest tile is 8
    //meaning the turn on the biggest tile is maxFractionTurnBased/8 * minwaitime
    public static int maxFractionTurnBased = 8 * 8 * 4;
    static int minWaitTime = 1000;


    //the time that is waited on the biggest tile

    public static int MaxWaitTime()
    {
        return (minWaitTime * maxFractionTurnBased);
    }
    public static int AbsoluteTime()
    {
        return Convert.ToInt32((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) % MaxWaitTime());
    }
    public static int TurnUnit()
    {
        return AbsoluteTime() / minWaitTime;
    }
    public static IObservable<int> GetTurnUnitObservable()
    {
        var turnDelay = 1 + (AbsoluteTime() / minWaitTime);
        //TODO the code in select is executed every time   
        return Observable.Interval(TimeSpan.FromMilliseconds(minWaitTime))
            //add how many UnitTurns have passed
            .Select(t => t + turnDelay)
            //the turn can never be bigger than max fraction
            .Select(t => Convert.ToInt32(t % maxFractionTurnBased))
             //sync TurnUnit to internal clock
            .Delay(TimeSpan.FromMilliseconds(minWaitTime -
             (Convert.ToInt32((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) % minWaitTime))));



    }
    public static int TileWaitTime(Tile tile)
    {
        return GetUnitTurnTimeScale(tile) * minWaitTime;
    }
    //doesn't work
    public static int GetTimeLeftInTurn(Tile tile, int absoluteTime=0)
    {
        UnityEngine.Debug.Log("tile wait time " + TileWaitTime(tile));
        UnityEngine.Debug.Log("AbsoluteTime() " + AbsoluteTime() % TileWaitTime(tile));
        UnityEngine.Debug.Log("tile scale() " + GetUnitTurnTimeScale(tile));
        UnityEngine.Debug.Log("system time " + Convert.ToInt32((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) % TileWaitTime(tile)));
        return TileWaitTime(tile) - (AbsoluteTime() % TileWaitTime(tile));
    }
    
    static int GetUnitTurnTimeScale(Tile tile)
    {
        return (maxFractionTurnBased / Mathf.Min(tile.GetAbsFraction(), maxFractionTurnBased));
    }
    public static bool GetTileTurnFromTurnUnit(int turnUnit, int playerSide, Tile tile)
    {
        return (turnUnit / GetUnitTurnTimeScale(tile)) % 2 == playerSide;
    }

    // based on the fraction of the parent tile give absolute counter of turn on
    // return tile turn based on absolute time







}
