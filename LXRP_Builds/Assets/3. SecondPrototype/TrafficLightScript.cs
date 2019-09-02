using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Animator))]
public class TrafficLightScript : MonoBehaviour
{
    [SerializeField] Animation _animButtonPress = null;
    [SerializeField] Animator _animaTrafficCont = null;
    [SerializeField] float _trafficCountdownDelay = 5.0f;

    [SerializeField] List<PathCreation.CarScript> _listTraffic = null;

    [SerializeField] int _amtCarToStopSpawn = 5;

    // Create event
    public delegate void StopCarSpawn(bool condition);
    public static event StopCarSpawn stopCarSapwn;

    public void OnTrafficButtonPress()
    {
        _animButtonPress.Play();
        _animaTrafficCont.SetBool("ButtonPressed", true);

        StartCoroutine(StartTrafficCountdown());
    }

    IEnumerator StartTrafficCountdown()
    {
        yield return new WaitForSeconds(_trafficCountdownDelay);
        _animaTrafficCont.SetBool("ResumeTraffic", true);
    }

    void AnimPedestrianCanWalk()
    {

    }

    void AnimPedestrianCannotWalk()
    {

    }

    void AnimResumeTraffic()
    {
        Debug.Log("----------------------Resuming Traffic");
        StartCoroutine(FreeTraffic());
    }

    IEnumerator FreeTraffic()
    {
        for (int i = 0; i < _listTraffic.Count; i++)
        {
            if (_listTraffic[i]._isStopped)
                _listTraffic[i].ResumeCar();
            _listTraffic.Remove(_listTraffic[i]);
            yield return new WaitForSeconds(1.0f);
        }
    }

    void AnimStopTraffic()
    {

    }

    void AnimResetParams()
    {
        _animaTrafficCont.SetBool("ResumeTraffic", false);
        _animaTrafficCont.SetBool("ButtonPressed", false);
    }

    public void AddToTrafficList(PathCreation.CarScript car)
    {
        if(!_listTraffic.Contains(car))
            _listTraffic.Add(car);

        if(_listTraffic.Count > _amtCarToStopSpawn)
        {

            stopCarSapwn?.Invoke(false);
        }
        else
        {
            stopCarSapwn?.Invoke(true);
        }
    }
}
