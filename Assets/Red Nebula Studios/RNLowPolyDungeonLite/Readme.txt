Thank you for downloading the Red Nebula Studios Low Poly Dungeon Lite set!

This is a free demo asset pack with examples from our full Low Poly Dungeon. If you like this demo, consider purchasing the full set! The full Low Poly Dungeon set contains 230 FBX models and a total of 294 prefabs, including static, animated, and lit props, decals, a scripted torchlight flicker effect, 5 particle effects, a full flythrough demo scene, and a bonus "Gloomy Dungeon" music file too.

Full Low Poly Dungeon: https://assetstore.unity.com/packages/slug/224090

Please let us know if you have any questions using the contact form on our website:

https://games.rednebulastudios.com/contact

If you use this pack in a finished product and wish to provide a credit, you may credit Red Nebula Studios. We'd appreciate it!

********************

DETAILS

The current version of this set is supported with Unity version 2019.4 and above. The main download supports the Built-in Render Pipeline. If you need support for the Universal Render Pipeline (URP), import the RNLowPolyDungeonLite_URP_2019.4.unitypackage file into your project instead. I highly recommend using Unity version 2020.2 or higher for URP, as that is when Screen Space Ambient Occlusion (SSAO) was introduced, and it makes props like these look a lot better!

The High Definition Render Pipeline (HDRP) is not currently supported, but you are free to import the files and update the materials as you see fit.

This modular low poly dungeon building set contains:

- 18 FBX models
- 1 color swatch texture
- 2 sprites used for particle effects and decals
- 19 total prefabs, including:
  - 12 static props
  - 2 activatable animated props
  - 1 lit props (torch)
  - 1 decal
  - 1 particle effect
- 2 demo scenes (1 asset showcase and one demo build, plus 1 supporting script)

Model poly counts:
- Min: 18
- Max: 543
- Avg: 152

********************

LICENSE

With the download of this free demo asset pack, you are granted a royalty-free license to use all assets contained herein for your own projects, commercial or non-commercial.

You are not permitted to:
- Claim credit for the creation of these asset files
- Resell, redistribute, or transfer the pack or individual asset files

********************

DEMO SCENES

Two demo scenes (Asset_Showcase and Demo_Build) are provided to preview all of the items in this set. When you press play in each one, props that are activatable will display a target cursor when you hover. Click to activate and de-activate them.

********************

PLACING OBJECTS

This set is modular and built so that the pieces fit together when moved incrementally. I strongly recommend using Unity's snap settings when building your scene. Walls are built on a 4m wide by 3m tall scale, so it's generally easiest to move objects by 1m at a time. Go to Edit -> Grid and Snap Settings and change the Move setting to 1. In your scene, activate grid snap by holding down the Ctrl key (or Command on a Mac) as you move the GameObjects around.

********************

MATERIALS

All of the props in this pack utilize a single color swatch texture for better performance. This also makes it very easy to adjust the color palette for your entire scene, by pulling the texture into an image editing program like Photoshop or GIMP and adjusting the colors.

The color swatch texture is used in one material, RNLowPolyDungeon_Main.

In addition, there are two other supplemental materials:

RNLowPolyDungeon_SpriteLit: Used for sprite decals, such as magic symbols and spiderwebs.
RNLowPolyDungeon_ParticleGlow: Used for particle systems. (Omitted in the URP package where the built-in ParticlesLit material is used instead.)

********************

ACTIVATED PROPS

Animated props are triggered using a boolean parameter called "Activated". To activate them via script, make sure you have access to the GameObjects Animator component, and update it using SetBool.

Example:

public class ActivateProp : MonoBehaviour
{
    Animator anim;

    void Start()
    {
	anim = GetComponent<Animator>();
    }

    void Update()
    {
	if (Input.GetKeyDown(KeyCode.O))
	{
	    anim.SetBool("Activated", true);
	}
	if (Input.GetKeyDown(KeyCode.P))
	{
	    anim.SetBool("Activated", false);
	}
    }
}

This simple example script would activate the object when you press O on your keyboard, and deactivate it when you press P. For GetComponent to work, this script would need to be placed on the same GameObject as the Animator component. For a more detailed example that also includes activating and deactivating lights and particle effects, take a look at the included ActivateProp.cs demo script.

Note that ActivateProp.cs is for demonstration purposes only in the example scenes and is not included in the prop prefabs. You can reuse it to suit the needs of your project or write an entirely new script, whichever works better.

********************

COLLIDERS

All mesh object prefabs are set up with colliders. Primitive colliders are used where possible. For very simple objects that primitives are unsuitable for, the existing mesh is used as a convex mesh collider instead. The archway piece uses a custom non-convex mesh collider for the opening.

********************

LIGHTING NOTES

The lit torch prefab contains a point light. This is fine if you have a very small number of these lights in your scene. However, point lights - and indeed, a large number of realtime light sources in general - are inefficient. if you intend to have a lot of light sources in your scene, you may wish to remove some or all of the point lights from individual prop prefabs and arrange your own lighting solution.

Because the props in this pack all use a single color swatch texture, the UV maps are very compact. This may result in you getting a warning message about overlapping UVs if you attempt to bake a lightmap with default settings. If you choose to use baked lightmaps, I recommend a lightmap resolution of at least 30. If you need to include very small objects in your baked lightmap for any reason, you may also need to increase their lightmap scale to avoid warnings. For more information about lightmaps, please see this video on Unity's YouTube channel: https://www.youtube.com/watch?v=KJ4fl-KBDR8

********************

IN CLOSING

Thank you once again for downloading this free demo asset pack. Red Nebula Studios is a small business run by myself (Sarrah) and my husband (Robert). Your support means the world to us!

Follow us:
@RNStudiosGames on Facebook and Twitter
https://games.rednebulastudios.com