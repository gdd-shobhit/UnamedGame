using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool dash;
		public bool pAttack;
		public bool hAttack;
		public bool tongue;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnDash(InputValue value)
        {
			DashInput(value.isPressed);
        }
		public void OnPAttack(InputValue value)
        {
			PAttackInput(value.isPressed);
        }

		public void OnHAttack(InputValue value)
		{
			HAttackInput(value.isPressed);
		}

		public void OnTongue(InputValue value)
		{
			TongueInput(value.isPressed);
		}


#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void DashInput(bool newDashState)
        {
			dash = newDashState;
		}

		public void PAttackInput(bool newAttackState)
		{
			pAttack = newAttackState;
		}

		public void HAttackInput(bool newHeavyAttackState)
        {
			hAttack = newHeavyAttackState;
        }

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		private void TongueInput(bool newTongueState){
			tongue = newTongueState;
		}
	}
	
}