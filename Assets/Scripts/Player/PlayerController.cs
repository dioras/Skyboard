using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;

    [Header("Move settings")]
    [SerializeField] private bool isMove = false;
    [SerializeField] private float moveSpeed = .2f;
    [SerializeField] private float rotateSpeed = .5f;
    [SerializeField] private float destructionForce = 50f;
    [SerializeField] private float currentHorizontalSensetivityEditor = 10f;
    [SerializeField] private float currentHorizontalSensetivity = 1f;

    [Header("Show effect")]
    [SerializeField] private float showEffectDuration = 2f;

    [Header("Board settings")]
    [SerializeField] private float maxAngleRotate = 14f;
    [SerializeField] private float rotateSpeedSkyBoard = 10f;

    [Header("Components")]
    [SerializeField] private Transform cameraTarget = default;
    [SerializeField] private Rigidbody playerBodyRigidbody = default;
    [SerializeField] private List<Rigidbody> ragdollRigidbodys = new List<Rigidbody>();
    [SerializeField] private TrailRenderer trail = default;
    [SerializeField] private Transform skyboardTransform = default;
    [SerializeField] private ParticleSystem particleSystemSpeed = default;
    [SerializeField] private List<SkinnedMeshRenderer> meshRenderersPlayerBody = new List<SkinnedMeshRenderer>(); 

    [Header("Actions")]
    public static Action<Transform> SetPlayerPositionAction = default;
    public static Action<Transform> SetPlayerBodyPositionAction = default;
    public static Action EffectShowPlayerAction = default;
    public static Action HidePlayerBodyAction = default;

    private float saveSpeed = default;
    private float horizontal = 0f;
    private int direction = 0;

    private Animator playerAnimator = default;
    private Sequence sequenceShow = default;

#region Animation
    private readonly int BoardIdle = Animator.StringToHash("board_idle");
    private readonly int Incline = Animator.StringToHash("incline");
    private readonly int Dance = Animator.StringToHash("dance");
    private readonly int JumpExit = Animator.StringToHash("jump_exit");
#endregion

#region getter/setter
    public Transform SkyboardTransform { get => skyboardTransform; }
#endregion

    private void OnEnable()
    {
        SetPlayerPositionAction += SetPlayerPosition;
        SetPlayerBodyPositionAction += SetPlayerBodyPosition;
        EffectShowPlayerAction += EffectShowPlayer;
        HidePlayerBodyAction += HidePlayerBody;

        GameManager.LevelStartAction += StartMove;
        GamePanelController.ShowReactionAction += ReactionPlayer;
    }

    private void OnDisable()
    {
        SetPlayerPositionAction -= SetPlayerPosition;
        SetPlayerBodyPositionAction -= SetPlayerBodyPosition;
        EffectShowPlayerAction -= EffectShowPlayer;
        HidePlayerBodyAction -= HidePlayerBody;

        GameManager.LevelStartAction -= StartMove;
        GamePanelController.ShowReactionAction -= ReactionPlayer;
    }

    private void Awake()
    {
        dependencyContainerSO.PlayerTransform = transform;
        dependencyContainerSO.PlayerController = this;
        playerAnimator = playerBodyRigidbody.transform.GetChild(0).GetComponent<Animator>();
        particleSystemSpeed.Stop();
        saveSpeed = moveSpeed;

        meshRenderersPlayerBody.ForEach((_renderer) => _renderer.material.DOFloat(1f, "_DissolveAmount", 0f));
    }

    private void Update()
    {
        if (dependencyContainerSO.InGame)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                var _mouseX = Input.GetAxis("Mouse X");
                horizontal += _mouseX * currentHorizontalSensetivityEditor * 1f;
                direction = _mouseX > 0 ? 1 : -1;
                if (_mouseX == 0)
                {
                    direction = 0;
                }
                if ((_mouseX < -.05f || _mouseX > .05f)
                    && dependencyContainerSO.IsTutorialShow)
                {
                    GamePanelController.ShowTutorialAction?.Invoke(false);
                }
            }
            else
            {
                direction = 0;
            }
#else
            if (Input.touchCount > 0)
	            {
                    var _mouseX = Input.touches[0].deltaPosition.x;
			        horizontal += _mouseX * currentHorizontalSensetivity;
                    direction = _mouseX > 0 ? 1 : -1;
                    if (_mouseX == 0)
                    {
                        direction = 0;
                    }
                    if ((_mouseX < -.05f || _mouseX > .05f)
                        && dependencyContainerSO.IsTutorialShow)
                        {
                            GamePanelController.ShowTutorialAction?.Invoke(false);
                        }
		        }
                else
                {
                    direction = 0;
                }
#endif
        }

        if (isMove)
        {
            if (dependencyContainerSO.InGame)
            {
                Rotate();
            }
        }

        BoardRotate();
    }

    private void LateUpdate()
    {
        if (isMove)
        {
            Move();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dependencyContainerSO.InGame)
        {
            if (other.tag.Equals(Tags.Barrier))
            {
                PlayerDestruction();
                StopMove(LevelResult.Lose);
            }
            else if (other.tag.Equals(Tags.Finish))
            {
                StopMove(LevelResult.Win);
            }
        }
    }

    private void EffectShowPlayer()
    {
        sequenceShow?.Complete();
        sequenceShow = DOTween.Sequence();
        meshRenderersPlayerBody.ForEach((_renderer) => sequenceShow.Join(_renderer.material.DOFloat(0f, "_DissolveAmount", showEffectDuration)));
        sequenceShow.OnComplete(() =>
        {
            trail.Clear();
            trail.enabled = true;
        });
    }

    private void HidePlayerBody()
    {
        meshRenderersPlayerBody.ForEach((_renderer) =>
        {
            _renderer.material.DOComplete();
            _renderer.material.DOFloat(1f, "_DissolveAmount", 2f);
        });
        trail.enabled = false;
    }

    private void SetPlayerPosition(Transform _transform)
    {
        transform.position = _transform.position;
        transform.eulerAngles = _transform.eulerAngles;
    }

    private void SetPlayerBodyPosition(Transform _transform)
    {
        playerAnimator.Rebind();
        playerBodyRigidbody.transform.SetParent(null);
        playerBodyRigidbody.isKinematic = true;
        playerBodyRigidbody.transform.position = _transform.position;
        playerBodyRigidbody.transform.eulerAngles = _transform.eulerAngles;
        playerAnimator.enabled = true;
        ragdollRigidbodys.ForEach((_rigidbody) => _rigidbody.isKinematic = true);
    }

    private void SetPlayerBodyPosition(Transform _transform, Action _action)
    {
        playerBodyRigidbody.transform.SetParent(null);
        playerBodyRigidbody.isKinematic = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(playerBodyRigidbody.transform.DOMove(_transform.position, .3f));
        sequence.Join(playerBodyRigidbody.transform.DORotate(_transform.eulerAngles, .3f));
        sequence.OnComplete(() => _action?.Invoke());
        playerAnimator.enabled = true;
        ragdollRigidbodys.ForEach((_rigidbody) => _rigidbody.isKinematic = true);
    }

    private void PlayerDestruction()
    {
        VibrationController.Vibrate(30);
        SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.PlayerFall);
        playerBodyRigidbody.transform.SetParent(null);
        playerBodyRigidbody.isKinematic = false;
        playerAnimator.enabled = false;
        //HidePlayerBody();
        ragdollRigidbodys.ForEach((_rigidbody) => { 
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = false;
            _rigidbody.AddForce(-Vector3.forward * destructionForce * Time.deltaTime, ForceMode.Impulse);
        });
    }

    private void ReactionPlayer()
    {
        //int[] _inclines = new int[] { InclineLeft, InclineRight };
        //playerAnimator.SetTrigger(_inclines[UnityEngine.Random.Range(0, _inclines.Length)]);
    }

    private void StartMove()
    {
        playerBodyRigidbody.transform.SetParent(skyboardTransform);
        playerBodyRigidbody.isKinematic = true;

        playerAnimator.SetBool(JumpExit, false);
        playerAnimator.SetBool(BoardIdle, true);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(.5f);
        sequence.Append(playerBodyRigidbody.transform.DOLocalMove(Vector3.zero, .8f));
        sequence.Join(playerBodyRigidbody.transform.DOLocalRotate(Vector3.zero, .5f));
        CameraController.SetTweenCameraPositionAction?.Invoke(cameraTarget.position + new Vector3(0f, 0f, 5f), cameraTarget.eulerAngles, () => { 
            CameraController.SetTargetAction?.Invoke(cameraTarget);
            horizontal = 0f;
            DOTween.To((_val) => moveSpeed = _val, 30f, saveSpeed, 5f);
            SoundManager.PlaySomeSoundContinuous?.Invoke(SoundType.BoardEngine, () => true);
            isMove = true;

            particleSystemSpeed.Play();
            VibrationController.Vibrate(30);
            UIController.ShowUIPanelAloneAction(UIPanelType.Game);
        });
    }

    private void StopMove(LevelResult levelResult)
    {
        dependencyContainerSO.InGame = false;
        direction = 0;
        skyboardTransform.localEulerAngles = Vector3.zero;

        if (levelResult == LevelResult.Win)
        {
            UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.None);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DORotate(Vector3.zero, 2f).OnComplete(() => CameraController.SetTweenCameraPositionAction?.Invoke(dependencyContainerSO.FinishCameraPosition.position, dependencyContainerSO.FinishCameraPosition.eulerAngles, null)));
            sequence.Join(DOTween.To((_val) => moveSpeed = _val, moveSpeed, 0f, 3.2f));
            sequence.OnComplete(() =>
            {
                SoundManager.StopSomeSoundNow?.Invoke(SoundType.BoardEngine);

                playerAnimator.SetBool(JumpExit, true);
                playerAnimator.SetBool(BoardIdle, false);
                playerAnimator.SetBool(Dance, true);

                StartCoroutine(WaitToHideSkyboard(.5f));

                isMove = false;
                horizontal = 0f;
                moveSpeed = saveSpeed;
                particleSystemSpeed.Stop();
                GameManager.LevelFinishAction?.Invoke(LevelResult.Win);
            });
        }
        else
        {
            playerAnimator.Rebind();
            SoundManager.StopSomeSoundNow?.Invoke(SoundType.BoardEngine);

            isMove = false;
            moveSpeed = saveSpeed;
            particleSystemSpeed.Stop();
            UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.None);
            GameManager.LevelFinishAction?.Invoke(LevelResult.Lose);
        }
    }

    private IEnumerator WaitToHideSkyboard(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        SkyboardController.HideBoardAtion?.Invoke();
        SetPlayerBodyPosition(dependencyContainerSO.PlayerFinishPoint, null);
    }

    private void Move()
    {
        transform.position -= transform.forward * moveSpeed * Time.deltaTime;
    }

    private void Rotate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0f, 0f, -horizontal)), rotateSpeed);
    }

    private void BoardRotate()
    {
        if (direction > 0)
        {
            var _targetRotation = Quaternion.Euler(new Vector3(skyboardTransform.localEulerAngles.x, maxAngleRotate, skyboardTransform.localEulerAngles.z));
            skyboardTransform.localRotation = Quaternion.Lerp(skyboardTransform.localRotation, _targetRotation, rotateSpeedSkyBoard);
            playerAnimator.SetInteger(Incline, 2);
        }
        else if (direction < 0)
        {
            var _targetRotation = Quaternion.Euler(new Vector3(skyboardTransform.localEulerAngles.x, -maxAngleRotate, skyboardTransform.localEulerAngles.z));
            skyboardTransform.localRotation = Quaternion.Lerp(skyboardTransform.localRotation, _targetRotation, rotateSpeedSkyBoard);
            playerAnimator.SetInteger(Incline, 1);
        }
        else
        {
            var _targetRotation = Quaternion.Euler(new Vector3(skyboardTransform.localEulerAngles.x, 0f, skyboardTransform.localEulerAngles.z));
            skyboardTransform.localRotation = Quaternion.Lerp(skyboardTransform.localRotation, _targetRotation, rotateSpeedSkyBoard * 2f);
            playerAnimator.SetInteger(Incline, 0);
        }
    }
}
