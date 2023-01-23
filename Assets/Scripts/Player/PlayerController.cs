using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("CamComponents")]
    [SerializeField] private Transform headPlayerTransfom; 
    [SerializeField] private Transform camTransform; 

    [Header("PlayerComponents")]
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private Rigidbody playerRig;

    [Header("PlayerSettings")]
    [SerializeField] private float speed, speedBonusOnRun, jumpForce;

    private Vector3 forcaPraFrente, forcaStrafe, forcaVertical; 

    private bool isRun; 

    [Header("CamSettings")]
    [SerializeField]private float maxCamRotationY; 
    [SerializeField]private float minCamRotationY;

    Vector2 mouseRotation; 

    [SerializeField] private float sensibilidade; 

    void Start()
    {
        CursorSettings(); 

        if(PlayerSettings.PlayerSensibilidade != 0f) sensibilidade = PlayerSettings.PlayerSensibilidade; 
    }
    
    void Update()
    {
        RotatePlayerAndMoveCam();  
        CheckInputs();  

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump(); 
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void CursorSettings()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }

    void CheckInputs()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRun = true; 
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRun = false; 
        }
    }

    void RotatePlayerAndMoveCam()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); 

        mouseRotation = new Vector2(mouseRotation.x + mouseInput.x * sensibilidade * Time.deltaTime,
                                    mouseRotation.y + mouseInput.y * sensibilidade * Time.deltaTime); 

        playerTransform.localRotation = Quaternion.Euler(0f, mouseRotation.x, 0f); 
        
        mouseRotation.y = Mathf.Clamp(mouseRotation.y, minCamRotationY, maxCamRotationY); 

        camTransform.localRotation = Quaternion.Euler(-mouseRotation.y, 0f, 0f);  
        headPlayerTransfom.localRotation = Quaternion.Euler(-mouseRotation.y, 0f, 0f); 
    }

    private float Speed()
    {
        if(isRun)
        {
            return speed + speedBonusOnRun; 
        }

        return speed; 
    }

    void MovePlayer()
    {
        float inputPraFrente = Input.GetAxis("Vertical"); 
        float inputStrafe = Input.GetAxis("Horizontal"); 

        forcaPraFrente = inputPraFrente * Speed() * transform.forward; 
        forcaStrafe = inputStrafe * Speed() * transform.right;
        
        Vector3 velocidadeFinal = forcaStrafe + forcaPraFrente + forcaVertical; 

        playerRig.MovePosition(playerRig.position + velocidadeFinal * Time.fixedDeltaTime); 
    }

    void Jump()
    {
        playerRig.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse); 
    }
}
