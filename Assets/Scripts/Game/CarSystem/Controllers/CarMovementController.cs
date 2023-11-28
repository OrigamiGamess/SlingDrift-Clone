using Config;
using Game.LevelSystem.LevelEvents;
using UnityEngine;

namespace Game.CarSystem.Controllers
{
    public class CarMovementController : MonoBehaviour
    {
        private CarSlingController _carSlingController;
        private CarDirectionController _carDirectionController;
        private CarCornerDetector _carCornerDetector;
        
        private TrailRenderer _driftEffect;
        
        public bool IsActive;

        private bool _movingActive;

        [SerializeField] private Transform passengerHolder;
        
        public void Initialize(CarSlingController carSlingController)
        {
            IsActive = false;

            _carSlingController = carSlingController;

            _driftEffect = GetComponentInChildren<TrailRenderer>();

            LevelEventBus.SubscribeEvent(LevelEventType.STARTED, ()=>
            {
                IsActive = true;
                _driftEffect.Clear();
            });
            LevelEventBus.SubscribeEvent(LevelEventType.FAIL, ()=>
            {
                IsActive = false;
            });
            
            _carCornerDetector = new CarCornerDetector(transform);
            _movingActive = true;
        }
        private void FixedUpdate()
        {
            if(!IsActive)
                return;
            
            CheckInput();
            Move();
        }

        private void Move()
        {
            if(!_movingActive)
                return;
            
            transform.Translate(transform.forward * (Time.deltaTime * GameConfig.CAR_SPEED),Space.World);
        }

        private void CheckInput()
        {
            _movingActive = true;
            _carCornerDetector.Detect();
            
            if(!_carSlingController.CheckAvailableSling())
                return;

            if (Input.GetMouseButton(0))
            {
                if (_carSlingController.OnDrifting(transform))
                {
                    if(!_driftEffect.emitting)
                    {
                        int randomPassenger = Random.Range(0, passengerHolder.childCount);
                        //Passenger passenger = objectPooler.SpawnFromPool("Passenger", transform, new Vector3(0f, 1.5f, 0f)).GetComponent<Passenger>();
                        Passenger passenger = passengerHolder.GetChild(randomPassenger).GetComponent<Passenger>();
                        passenger.transform.SetParent(null);
                        passenger.KinematicBody(false); //turn on ragdoll
                        passenger.ApplyRagdollEffect(_carSlingController.TargetSling.GetDirection());
                    }

                    _movingActive = false;
                    _driftEffect.emitting = true;
                }
            }
            else
            {
                _carSlingController.OnDriftingFinished(transform);
                _driftEffect.emitting = false;
            }
        }
    }
}
