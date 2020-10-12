/**
 * Copyright (c) 2017 The Campfire Union Inc - All Rights Reserved.
 *
 * Licensed under the MIT license. See LICENSE file in the project root for
 * full license information.
 *
 * Email:   info@campfireunion.com
 * Website: https://www.campfireunion.com
 */

using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace VRKeys {

	/// <summary>
	/// Example use of VRKeys keyboard.
	/// </summary>
	public class DemoScene : MonoBehaviour {

		/// <summary>
		/// Reference to the VRKeys keyboard.
		/// </summary>
		public Keyboard keyboard;

        public GameObject combine;
        public GameObject control;


		/// <summary>
		/// See the following for why this is so convoluted:
		/// http://referencesource.microsoft.com/#System.ComponentModel.DataAnnotations/DataAnnotations/EmailAddressAttribute.cs,54
		/// http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx/
		/// </summary>
		private Regex emailValidator = new Regex (@"^[A-Za-z0-9]+$", RegexOptions.IgnoreCase);

		/// <summary>
		/// Show the keyboard with a custom input message. Attaching events dynamically,
		/// but you can also use the inspector.
		/// </summary>
		private void OnEnable () {
			// Automatically creating camera here to show how
			GameObject camera = new GameObject ("Main Camera");
			Camera cam = camera.AddComponent<Camera> ();
			cam.nearClipPlane = 0.1f;
			camera.AddComponent<AudioListener> ();

			// Improves event system performance
			Canvas canvas = keyboard.canvas.GetComponent<Canvas> ();
			canvas.worldCamera = cam;

			keyboard.Enable ();
			keyboard.SetPlaceholderMessage ("Please enter your car name");

			keyboard.OnUpdate.AddListener (HandleUpdate);
			keyboard.OnSubmit.AddListener (HandleSubmit);
			keyboard.OnCancel.AddListener (HandleCancel);
		}

		private void OnDisable () {
			keyboard.OnUpdate.RemoveListener (HandleUpdate);
			keyboard.OnSubmit.RemoveListener (HandleSubmit);
			keyboard.OnCancel.RemoveListener (HandleCancel);

			keyboard.Disable ();
		}


        /// <summary>
        /// Press space to show/hide the keyboard.
        ///
        /// Press Q for Qwerty keyboard, D for Dvorak keyboard, and F for French keyboard.
        /// </summary>
        private void Update () {
			if (Input.GetKeyDown (KeyCode.Space)) {
				if (keyboard.disabled) {
					keyboard.Enable ();
				} else {
					keyboard.Disable ();
				}
			}

			if (keyboard.disabled) {
				return;
			}

			if (Input.GetKeyDown (KeyCode.Q)) {
				keyboard.SetLayout (KeyboardLayout.Qwerty);
			} else if (Input.GetKeyDown (KeyCode.F)) {
				keyboard.SetLayout (KeyboardLayout.French);
			} else if (Input.GetKeyDown (KeyCode.D)) {
				keyboard.SetLayout (KeyboardLayout.Dvorak);
			}
		}

		/// <summary>
		/// Hide the validation message on update. Connect this to OnUpdate.
		/// </summary>
		public void HandleUpdate (string text) {
			keyboard.HideValidationMessage ();
		}

		/// <summary>
		/// Validate the email and simulate a form submission. Connect this to OnSubmit.
		/// </summary>
		public void HandleSubmit (string text) {
			keyboard.DisableInput ();

			if (!ValidateEmail (text)) {
				keyboard.ShowValidationMessage ("Please enter English or Number");
				keyboard.EnableInput ();
				return;
			}

            StreamReader sr = new StreamReader("Assets/Resources/CarList/list.txt");
            List<string> carList = new List<string>();
            string test = sr.ReadLine();
            while (test != null)
            {
                carList.Add(test);
                test = sr.ReadLine();
            }
            sr.Close();

            foreach (string car in carList)
            {
                if(text.Equals(car))
                {
                    keyboard.ShowValidationMessage("Duplicate name");
                    keyboard.SetText("");
                    keyboard.EnableInput();
                    return;
                }
            }
            StreamWriter sw = new StreamWriter("Assets/Resources/CarList/list.txt", true);
            sw.WriteLine(text);
            sw.Close();

            combine.GetComponent<Combine>().fuck();
        }

		public void HandleCancel () {
			Debug.Log ("Cancelled keyboard input!");
            this.gameObject.SetActive(false);
            control.SetActive(true);

            GameObject camera = GameObject.Find("Main Camera");
            GameObject.Destroy(camera);

        }

        /// <summary>
        /// Pretend to submit the email before resetting.
        /// </summary>
        //private IEnumerator SubmitEmail (string email) {
        //	keyboard.ShowInfoMessage ("Sending lots of spam, please wait... ;)");

        //	yield return new WaitForSeconds (2f);

        //	keyboard.ShowSuccessMessage ("Lots of spam sent to " + email);

        //	yield return new WaitForSeconds (2f);

        //	keyboard.HideSuccessMessage ();
        //	keyboard.SetText ("");
        //	keyboard.EnableInput ();
        //}


		private bool ValidateEmail (string text) {
			if (!emailValidator.IsMatch (text)) {
				return false;
			}
			return true;
		}

        

    }
}