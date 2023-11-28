using System.Collections;
using UnityEngine;

/*
 * Description: Script that activates and deactivates animated, lit, and particle effect props in the RNLowPolyDungeonLite set. It is included for demonstration purposes,
 * but can be used as an example of how to set up activation of props in your own project. Note that this script is only included on props in the demo scenes and is not
 * included in the prop prefabs.
 * Author: Sarrah Wilkinson - Red Nebula Studios
 * Created Date: 6/4/2022
 * Last Modified Date: 6/4/2022
 * Copyright: (c) 2022 Red Nebula Studios - Included with the RNLowPolyDungeon set for royalty free use by purchasers for finished games and other Unity projects.
 * Not for individual resale or redistribution.
 * Questions? Use the contact form at http://games.rednebulastudios.com/ to get in touch!
 */

namespace RNLowPolyDungeonLite
{
    [RequireComponent(typeof(BoxCollider))]
    public class ActivateProp : MonoBehaviour
    {
        [Tooltip("Is the prop currently in its active state? Make sure to set true if you want a prop (such as a torch) to be active at runtime.")]
        [SerializeField] bool propIsActive = false;
        [Tooltip("If the prop has a Light component, this controls how fast to ramp up or down the light intensity when it is turned on or off.")]
        [SerializeField] float lightActivateSpeed = 2f;
        [Tooltip("The cursor graphic to display when the mouse is hovered over an activatable object.")]
        [SerializeField] Texture2D cursorTexture = null;
        [Tooltip("The location of the cursor's target. Example: 16 x 16 would be the center of a 32px x 32px cursor.")]
        [SerializeField] Vector2 cursorTarget = new Vector2(0f,0f);

        Animator anim;
        ParticleSystem[] particleSystems;
        Light ltSource;

        float ltDefaultIntensity;
        Coroutine lightCoroutine;

        void Start()
        {
            // Retrieve the appropriate component(s) from the prop. Because activated props are contained within a parent game object, GetComponentInChildren is used
            // here instead of GetComponent. One prop (the candle floor sconce) contains multiple particle systems, so they are retrieved as an array instead of a
            // single field.
            anim = GetComponentInChildren<Animator>();
            particleSystems = GetComponentsInChildren<ParticleSystem>();
            ltSource = GetComponentInChildren<Light>();

            // If the prop contains a light, this captures its default intensity so it can be turned on and off.
            if (ltSource)
                ltDefaultIntensity = ltSource.intensity;

            // If none of the activatable components are found on the object or its children, a warning is passed to the console.
            if (!anim && particleSystems.Length == 0 && !ltSource)
                Debug.LogWarning("ActivateProp script can not find any activatable components on object: " + gameObject.name);
            
            // Set the prop to match its activation status at run time.
            else
                Activate(true);
        }

        /*
         * Props using the ActivateProp.cs script have a single box collider on the parent object. This is ONLY in the demo scenes and is not contained
         * in the prefabs. The following three methods (OnMouseEnter, OnMouseExit, and OnMouseDown) will only work with a collider on the same game object
         * as the script. Please be aware this is NOT the best way to activate props, but the better method requires the use of layers and cannot be done
         * within a unitypackage file sold through the asset store.
         * 
         * The better way to activate props would be to place the object(s) with the collider you want to be clickable on a layer called 'Clickable', and
         * use a raycast with a layer mask. That way, you do not have to rely on the script being on the same layer as the collider at all.
         * 
         * Read more about raycasts here: https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
         */
        void OnMouseEnter()
        {
            // When the mouse enters the game object's collider, change the cursor.
            Cursor.SetCursor(cursorTexture, cursorTarget, CursorMode.Auto);
        }

        void OnMouseExit()
        {
            // When the mouse exits the game object's collider, return the cursor to default.
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        void OnMouseDown()
        {
            // When the game object's collider is clicked, toggle the prop's activation status.
            Activate();
        }

        /// <summary>
        /// Toggle the activation status of the prop.
        /// </summary>
        /// <param name="setStartingActivation">When this method is first called as the game start, set true. Otherwise, disregard.</param>
        void Activate(bool setStartingActivation = false)
        {
            // Toggle the prop's activation status boolean to the opposite of its current setting. (Leave as-is if the game is just starting.)
            if (!setStartingActivation)
                propIsActive = !propIsActive;

            // If the prop has an animated component, this activates or deactivates it.
            if (anim)
                anim.SetBool("Activated", propIsActive);

            // If the prop has any particle system components, this activates or deactivates them.
            foreach (var ps in particleSystems)
            {
                if (propIsActive)
                {
                    if (setStartingActivation)
                    {
                        // Always prewarm looping particle effects if they are set active at runtime, even if they are not set to prewarm by default.
                        bool isPrewarmed = ps.main.prewarm;
                        var main = ps.main;
                        main.prewarm = true;
                        ps.Play();
                        main.prewarm = isPrewarmed;
                    }
                    else
                        ps.Play();
                }
                else
                {
                    ps.Stop();
                    if (setStartingActivation)
                        ps.Clear();
                }
            }

            // If the prop has a light component, this turns it on or off. During runtime, this is handled by a pair of coroutines that allow the light's intensity
            // to ramp up or down at the speed set in the field lightActivateSpeed.
            if (ltSource)
            {
                if (setStartingActivation)
                {
                    if (propIsActive)
                        ltSource.intensity = ltDefaultIntensity;
                    else
                        ltSource.intensity = 0f;
                }
                else
                {
                    if (lightCoroutine != null)
                        StopCoroutine(lightCoroutine);
                    if (propIsActive)
                        lightCoroutine = StartCoroutine(TurnLightOn());
                    else
                        lightCoroutine = StartCoroutine(TurnLightOff());
                }
            }
        }

        // Coroutine that ramps the light's intensity up as it is being turned on.
        IEnumerator TurnLightOn()
        {
            bool isDone = false;

            while (!isDone)
            {
                ltSource.intensity += lightActivateSpeed * Time.deltaTime;
                if (ltSource.intensity >= ltDefaultIntensity)
                {
                    ltSource.intensity = ltDefaultIntensity;
                    isDone = true;
                }
                yield return null;
            }
        }

        // Coroutine that ramps the light's intensity down as it is being turned off.
        IEnumerator TurnLightOff()
        {
            bool isDone = false;

            while (!isDone)
            {
                ltSource.intensity -= lightActivateSpeed * Time.deltaTime;
                if (ltSource.intensity <= 0)
                {
                    ltSource.intensity = 0;
                    isDone = true;
                }
                yield return null;
            }
        }
    }
}



