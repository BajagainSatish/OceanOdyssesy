Help Me Place! Thank you for purchasing Help Me Place: An object placement solution for your Editor!

There are a number of Tutorial scenes and Demos available in the Examples Folder.
If you have any questions, feature requests or if you have found a bug then please send me an email at:
	irishjohngaming@gmail.com
- Or contact me directly on Discord:
	JohnDong#6317

Quick Start Guide
	Help me Place was created with a view of ease of use, but there is some initial setup required. 
	There is a short tutorial video on the Asset store available, or you can search youtube for 'Help Me Place tutorial'.
	* Please note, for URP / HDRP you will need to update the Blueprint material. Please see: URP/HDRP Setup. 

	The examples folder comes complete with a number of Voxel models included. There is also a prefab named "HelpMePlace_WithExamplesLoaded" which has all the voxel prefabs added. 

		To create an initial 'playground' where you can play around with all the features safely:
			Create a new scene.
			Drag the prefab 'HelpMePlace_WithExamplesLoaded' into the scene.
			Create a Terrain in the scene.
			Select the HelpMePlace prefab, and press the Enable button.
			Click into the scene View, you should notice that you are now using the 'Hand tool'.

			You should now be able to see the blueprint of a small house as you move your mouse across the terrain. 
			If you are unable to see the blueprint, please ensure the Terrain is on the Default Layer. Alternativly, you can choose the appropriate layer in the HelpMePlace Component on the HelpMePlace_WithExamplesLoaded. 

			You are now ready to use Help Me Place! Feel free to experiment with all the features available. Go ahead and hit the Space Bar whilst hovering your mouse over the Terrain to place your first prefab!

			Changes you make to the "HelpMePlace_WithExamplesLoaded" prefab will not effect the main "HelpMePlace" prefab. And you can always reset it by re-importing until you are comfortable with the asset. 

			Help me Place also comes with an Extended Flycam in the examples folder for your convinience, you can choose to bring this into your scene if you wish to fly around the scene at run time. 
			
			This concludes the Quick Start Guide. All the features available in the Help me Place system have tooltips associated with them. Hover each feature to read more about what they do. 

URP/HDRP Setup
	When using HDRP / URP some additional setup is required in some cases, to make the blueprint correctly visible.
	You may get an initial error after importing the asset when using these pipelines. You can just ignore the error as it is referring to the thing we are about to fix.

	Note: A screenshot of these settings is included in the HelpMePlace > Helpers - HDRP.URP.BlueprintMaterialSettings.png

	Navigate to HelpMePlace > Helpers and select the HMP_BlueprintMaterial
	Change the Shader for this material to be Particles / Lit
	Change the Surface Type to Transparent.
	Change the Color to a blue color of your choosing, and reduce the Alpha to around 150.

Functional Documentation
	Initial Setup
		In order to use Help Me Place effectivly I recommend you familiarise yourself with the Example Scenes, and try the Quick Start guide. 

		Please proceed by dragging the Help Me Place prefab in the root of the HelpMePlace asset directory into your scene.
		Alternativly you can add the HelpMePlaceSystem.cs script to an existing gameobject.

		Help Me Place requires at least one Prefab Group, with at least one Prefab or GameObject added to it in order to be activatable. 

		Create new prefab group from Folder
			Prefab groups can be added manually, or by selecting a folder. Any prefabs found within this folder and any subfolders will then be added as prefab groups automatically.
			Prefab groups are reorderable and customisable. 
			
		To add a Prefab Group Manually, hit the Plus button under "Prefab Groups" in the Help Me Place component. 

		Prefab groups are expandable rows, with a number of fields.
			Group Name
				This is useful for labelling and organising your prefab groups. This will also be shown in the scene UI when using HelpMePlace.
			Random From Row
				Use this checkbox if you would like the system to choose a random prefab from the list each time one has been placed. This is very useful for things like Rocks and Trees.
			Rotate Around Axis
				The axis in which rotations are applied to the objects being placed.
			Scale Range Min & Scale Range Max
				The values entered in this range will apply a scale to any object placed in this prefab group. 
				For example, if you enter 2 minimum, and 5 maximum, every object being placed will have its scale changed between 2 and 5 randomly. 
				This is very useful for Rocks and Trees for example. 
				A smaller than 1 value will decrease the size of the objects being placed. 
			Desired Parent
				If you would like anything spawned from this group to have a specific transform as a parent, you can add that transform here. 
		
		If you have chosen to add manually:
			You can now start adding prefabs to this group. Note: These can be prefabs, OR any gameobject in the current scene.
			Use the plus button below to begin adding prefabs to the list. You can choose a maximum of 13 prefabs per group as per version 0.1.

		At this point there is no other setup required. You can begin using the system by hitting 'Enable'.

		Pay attention to the console, as any validation errors or problems will appear there at this stage.

		All controls are configurable, but to demonstrate and test that the set up is complete, you can hover over the terrain in your scene view, and hit the Space Bar to place the prefab. If you added more than one prefab, you can hit D or A to move to it, and hit space bar again to place it. 

		Everything in Help Me Place is hotkeyed. So you can now try out the 'Random from this row' by hitting T. This will only work if you have more than one prefab in your group. Now hit the space bar a couple of times and you should see that the system is changing the selected prefab randomly.

		If you notice that some of your prefabs/gameobjects are showing underground or too high off the ground, you can adjust their Sink Amount. This is a very handy feature that allows you to change the height from the ground that the prefab / gameobject will settle at. To change the sink amount, you can hold CTRL at any time whilst hovering the terrain, and moving the mouse. 

		If you notice the sinking sensitivity is too low or high, you can change that in the Help Me Place configuration on the HelpMePlace gameobject. More about Sinking in the core systems explanations.
	
	Physics-in-editor
		This feature was added in v1.3 and has a few limitations.

		In order to use this feature, you must disable Animate Placement under HMP System.
		This will stop the system from animating the item falling into place. 
		Instead, HMP allows you to spawn objects with rigid bodies anywhere - using the normal placement method, and then simulate physics within the scene view, so as they naturally fall using physics. 

		This is particularly useful if placing items with rigidbodies. 

		Please note that this is an experimental feature.
		The simulation will efect ALL rigidbodies in the scene. Not just the items created by Help Me Place.

		To simulate physics, hold V. Remember, Animate Placement must be disabled under HMP System. 

	Fence Placement System
		Added in v3, the Fence Placement system will help you draw lines of any prefab, be it fences, houses, trees. Anything can be spread in a line by holding C, moving your mouse, and releasing C. 
		To increase the gap between fences, simply click / right click to decrease the gap.

		If you are noticing that your fences are not facing the correct direction, ensure the forward of your prefab is set correctly. Sometimes a prefab may look like its facing a certain direction, but in actuality its facing another direction incorrectly. 
		To fix this - you can reparent the object & add the reparented prefab to Help Me Place. 
		
		If you would like for your fences to have a different angle applied - visit HMPFencePlacement and change the line that looks like: 
			rot = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(0, 90, 0); 
		to whatever angle you desire. This translation is applied to all fences and preview items. 

	Visualize Helpers
		Version 3 of HMP comes with multiple visualizers to help you see the density and occurances of prefab usage. To enable these, hit enable on the Prefab Group Visualization section of the prefab groups.
		Your scene will then be filled with gizmos showing your prefabs. You can use the 'Visualizer Shape' & 'Visualizer Shape Size' to customise how they are displayed. You can choose 'auto refresh' to keep the visualiser active & refreshing, but this may cause slowdowns in the editor (based off how many items you have in your scene)

	Event Hooks
		You can now expand a section with Event hooks where you can inject your own logic to important system events.

	Core Systems and Functions
		Hotkey System
			As of Version 3, you can now re-enable the tool after its been activated at least once in the scene by using KeyPad 5. You can also assign hotkeys to each individual prefab group. This will select that group based off hotkey pressed.
			I recommend only using the keypad number keys for this. 

		Prefab Groups
			These are the core of the system. It is important that these are populated correctly, with items that have correct rotations already. HelpMePlace operates with a 'World Up is always UP' premise. If the objects added to the system are not rotated correctly, the system will place them as they were added. Please see the tutorial video for explanations on this. 

			Properties
				Group Name
					This is useful for labelling and organising your prefab groups. This will also be shown in the scene UI when using HelpMePlace.
				Random From Row
					Use this checkbox if you would like the system to choose a random prefab from the list each time one has been placed. This is very useful for things like Rocks and Trees.
				Rotation To Normal
					If checked, everything in this group will align itself to the normal of the item it is being placed on. Useful for uneven terrain, fences, that kind of thing.
				Desired Parent
					If you would like anything spawned from this group to have a specific transform as a parent, you can add that transform here. 
				Prefabs
					Any prefabs or gameobjects from the current scene that you would like to spawn should be added to this list. 
				Color
					This is another visual que to how the prefab group is represented in the tool & visualizer.

		Configuration
			Most items are configurable in Help Me Place. These are the common configurable items. Most of these follow the hotkey standard. Check the KeyBinds section below for more details on that. 
			
			Ground Layer
				This is the layer that the Help Me Place system will effect. In most cases, if creating a city or village, this will be whatever layer your terrain is on. If you would like to place things on top of eachother, or you'd like to place things on a 'table' for example, you should ensure that layer is added here. 

			Rotation
				Rotation in Help Me Place is where the most usefulness of the tool lies. You can choose from a number of rotation modes quite easily and control them using the hotkeys for fast and easy placement of objects with a large range of use cases covered. 

				The Rotation Mode can be changed by hotkey inside scene view. See KeyBinds.

				Rotation Mode
					Smooth
						This mode will smoothly rotate your blueprint when holding the Left or Right mouse buttons. To increase / decrease the speed of rotation use the Rotation Smooth Sensitivity slider below. 

					Snap
						This will snap the rotation by the angle specified in the Rotation Fixed Amount below. 

					Fixed
						The fixed rotation is updated by all other tools. If you would like to ensure mouse clicks will not rotate 
						the blueprint, use this function to lock the rotation to the blueprints current rotation. 
						Useful if you want to make all houses on a street have the exact same rotation etc.

					Random
						The random rotation mode will change the rotation randomly of the blueprint after every placement.

					Look Towards
						Makes the forward vector of the blueprint face the Look At Position specified. 

					Look Away From
						Makes the forward vector of the blueprint face away from the Look At Position specified. 						
				Rotation Snap Amount
					Used by the Rotation Mode - Snap.

				Rotation Fixed Amount
					Used by the Rotation Mode - Fixed.

				Rotation Smooth Sensitivity
					Used by the Rotation Mode - Smooth.

				Look At Position
					Used by the Rotation Modes - Look Towards and Look Away From.

			After Placement Options
				All Spawned Object Parent
					If an object is specified here, it will be used to parent all spawned objects. 

				Make Prefab Static
					If you would like the prefabs to be made static after placement, enable this option.

			Preview Images
				Preview Image Size
					This is the size of the UI shown in the editor, relative to the Preview Images for each prefab. 

				Show Keybinds
					To show or hide the Keybinds visibility under each feature in the UI, use this checkbox.

			Helper Controls - Sinking
				ALT Adjusts Sink Amount
					This feature allows you to adjust the SinkAmount of each prefab by holding the ALT button, and moving the mouse in an upward / downward motion.

				ALT Adjustment Sink Amount
					This controls the sensitivity of the sink amount adjustments when holding ALT.

		HMP System
			Map Projections
				These are Projectors that show an image projected against the Ground Layer. These can be very handy to visualise layouts and using Help Me Place's blueprint system to match the designs or desired layouts.

				Overlay Map Game Objects
					These are the gameobjects under HelpMePlace > Helpers MapProjection 1 - 4.
					These can be toggled on/off via hotkey. 
					They will automatically use the Ground Layer specified to project images onto the ground as assistance to layout / level design.
					To change the projections, select a map projection from the list, and update the Material's Cookie. 
						These materials are located in HelpMePlace > Helpers > MapProjectionHelperMat# - numbered 1-4

			Gizmos
				Gizmo Color
					For Gizmos such as the "Look At Position" sphere color updates, you can feel free to update this color. 

			Logging
				Show Activity Notification
					By default, HelpMePlace will give you active / deactive notifications to notify you about when the tool becomes active / deactivated. This is purely a visual que to go alongside the Unity Editor UI.
					If the component is 'collapsed' or the Gizmos feature is turned off, no Unity Editor UI will show, and the Hand Tool will be forced each frame. So this notification can remind you to check these things incase of unexpected behavior.

			Placement Animation
				Animate Placement
					If you would like to disable the 'falling' into place animation, untick this checkbox. This can be especially useful if you are stacking prefabs on top of eachother.

			Textures for drawing editor GUI
				This is a list of textures that are used by the internal system to draw the Editor UI. 

		KeyBinds
			N.B. These Hotkeys / Keybinds will only function if the Scene View is in Focus. 

			All functional keybinds for the system are customisable. 
			Please visit HMP_KeyBinds.cs to update keybinds. 

			General
				ESC
					This hotkey will disable the HelpMePlace feature. 

				Shift & Spacebar
					This hotkey is provided by Unity. It will toggle maximise the currently selected editor window. I recommend using it on the Scene View when using HelpMePlace.

			Configurable (HMP_KeyBinds.cs)
				ToggleOverlayMap
					Toggles active flag on all Map Projections. Default: Tab

				ExitTool 
					Disables the HelpMePlace asset. Default: Escape

				PlaceObject
					Spawns the selected object at the current blueprint position. Default: Space

				SelectionMoveLeft
					Moves your Prefab selection Left, if used at the leftmost item in a PrefabGroup, it will loop to the rightmost item. Default: A

				SelectionMoveRight
					Moves your Prefab selection Right, if used at the rightmost item in a PrefabGroup, it will loop to the leftmost item. Default: D

				SelectNextGroup
					Selects the next prefab group.

				SelectPreviousGroup
					Selects the previous prefab group.

				CycleRotationModes
					Loops through the available rotation modes.

				SetLookAtPosition
					Sets the current 'Look At Position' for use by the Look towards / Look away from Rotation Modes.

				RandomFromRowToggle
					Toggles the Random From Row flag for the current Prefab Group.

Code Documentation
	The code is commented in full but here is a quick overview of how the system ties together. 
	Updates to any of these classe are not supported. You can modify them, but it may result in unexpected behavior. 

	HelpMePlaceSystem.cs
		This is the core of the system. It is responsible for management and direction for all other classes within the system. This is the part of the system that draws the Editor UI, as well as subscribing to Scene events and initialisation.

	Configuration
		HMPConfiguration
			This is the default serialized class for all properties that are safe to be configured by the user.

		HMPKeyBinds
			This class holds all keybinds and is safe to modify. All binds are bound to keyboard, so selecting a Mouse button will require other changes. 
			See KeyBinds described above for more.

	Dependants
		HMPBlueprintHandler
			This class is responsible for all updates to the blueprint. Any movement, modification or update to the current selected object/group are reflected within this class.

		HMPConstants
			This holds all constant values and defaults used by HelpMePlace.

		HMPEnums
			Holds all enums used by HelpMePlace.

		HMPHelperFunctions
			This holds many commonly used functions used by many HelpMePlace classes.

		HMPPrefabGroup
			This is the serialized class used by the HelpMePlace system to hold all the prefab groups and prefabs, which are then later updated to Reorderable.

		HMPSelectedPrefabHandler
			This class is responsible for the selection and traversal through the prefab groups and their prefabs.

		HMPSpawnedTransform
			This class is responsible for the updating and handling of any updates to the spawned prefabs / transforms / gameobjects.

		HMPValidator
			This class holds all potential errors that may occur in HelpMePlace. 

	Settings
		HMPSystemSettings
			This class holds all system settings used by HelpMePlace to draw the editor scene UI and gizmos. 

	Editor
		HelpMePlaceEditor
			This class holds all editor functionality and inspector gui updates for the HelpMePlace component. 

FAQ
Q.	What is the quickest way I can start playing around with this.
A.	Open a demo scene, ensure Gizmos are enabled in the Scene view, and activate the HelpMePlace_WithExamplesIncluded Gameobject. 
	Start hitting space whilst the scene view is in focus (click), and mouse is over the terrain.

Q.	The HelpMePlace system is not activating.
A.	You must ensure you have Prefab Groups with prefabs added to them in order to use the system. Check the console for more details.

Q.	Can I add particle effects or waypoints using this tool?
A.	Yes, any gameobject can be added to a prefab group. 

Q.	What if the gameobjects have scripts?
A.	Any gameobject, with any components/scripts can be used. 

Q.	I can't see the blueprint.
A.	Please check your ground layer is set to the same as the surface you are trying to use. 
	Please ensure the object that is your ground has a collider. 
	Please ensure you have added at least one prefab group, and at least one prefab to the HelpMePlace system.
	Please ensure you have activated the tool.
	Please check that Gizmos is Active in the Scene View.

Q.	I can see the blueprint, but no UI.
A.	Please ensure you have Gizmos active in Scene View. 
	Please ensure the component is expanded in the HelpMePlace Gameobject. 

Q.	I don't want the MapProjection feature.
A.	You can disable this feature using the Tab key, alternativly, you can delete the Helpers gameobject or delete the MapProjection gameobjects themselves. 
	You can also just turn off the projector components within the MapProjection gameobjects.

Q. 	The LookAt gizmo is hard to see in my scene.
A.	You can configure the gizmo color in the HelpMePlace component > HMP System > Gizmos > Gizmo Color.

Q.	The preview images are too small.
A.	You can increase the size of the preview images in the Configuration > Preview Images > Preview Image Size property.

Q.	How do I fullscreen the Scene View.
A.	This is a handy Unity Feature, use Shift & Space bar to toggle between fullscreen/windowed. 

Q.	I accidently updated my object sink amount using ALT.
A.	You can reset the sink amount to zero using the HelpMePlace component, there is a button at the bottom of the configuration section. 
	Also, you can disable this feature if you're happy with your sink amounts. Just turn off Configuration > Helper Controls - Sinking > Ctrl Adjusts Sink Amount. 

Q.  The blueprint keeps rotating when in the look at / look away from modes. 
A.	Your rotation axis is set to 'All'. This is expected behavior. 

ChangeLog
3.0.0
- Major update potentially introducing some breaking changes. Many QOL improvements & new functionality.
	- Re-enable hotkey
		- You can now re-enable HMP by pressing keypad 5! This means, you never have to exit the full-screen scene view (shift & space bar) - making your workflow much much easier.
		- As always, you can hit Esc to disable HMP temporarily if you wish to use the move tool for example, then quickly re-enable using keypad 5 to get back to working with HMP.
	- Fence tool
		- Place rows of any prefab you wish (Hold & Release C) & customise the spacing between them by clicking. Works on variable heights & any prefab you wish. Especially useful when placing fences / hedgerows.
	- Group Colors
		- Choose colors for your prefab groups to easily identify them.
	- Visualizer
		- A brand new feature that allows you to see all the prefabs and their densities inside the scene. This is especially useful in wireframe mode where you can see every group object you've placed so far.
		- Extremely useful to identify places that may need more props / places that have too many of certain groups. 
	- Group Hotkeys
		- One of the most requested features. Especially useful for users with high amounts of groups.
		- You can now assign hotkeys to your prefab groups! 
		- I recommend using the keypad for these. 
		- Example: Assigning keypad 1 for houses, 3 for castles, 8 for fences. Tapping these will select that group, no more scrolling through high amount of groups to get to the set you want :)
	- Event hooks
		- You can now hook to events using the exposed unityevents. 
		- Add any custom code to events like 'HMP Activated / deactivated' or 'On Object Placed' for example.
		- Useful if you want to scatter more objects of the same type around the area you have placed into. 
			- N.B. This 'scatter' feature has so many potential usecases that I feel this is better than providing a bespoke solution. 
			- We may revisit this in the next version if there is enough ask for it.
	- Lineups
		- Line up all prefabs in your groups!
		- Organises every prefab that you have loaded into Help Me Place in the current scene.
		- Allows you to do quick edits to your prefabs, without having to find them & bring them into a scene manually. 
		- This can help massivly if you notice something wrong with forward vectors, or orientation of prefabs that you're placing. 
		- Remember, this is adding the prefabs to the scene - changes will still need to be applied to the prefab (you can use the override feature in unity for handiness).
	- Scale refreshing
		- A nice quality of life change example - changes to scale min/max will now apply in real time to the blueprint.
	- Internal Changes
		- Alot of refactoring went into this update. 
		- I have been using HMP heavily in my own projects and been listening to your feedback. 
		- This update has driven many changes internally & improved the usability, user friendliness and performance 10 fold.
	- Documentation Updates

2.0.0
- Major update including many quality of life improvements bug fixes & interface updates. 
	- New Rotate to normal feature.
		- Matches the normal of the item you are hitting, enabling 2d support, and any shape of ground mesh.
		- Reworked the placement target system so that it works with underside and inverse positions.
	- Editor updates
		- New tabbed editor layout seperating all the relevant sections of the tool for easier navigation
	- Prefab group utilities
		- Split your groups and reset sink amounts
		- Reset and clear your groups, so that copying is no longer as tedious
		- Line up all prefabs per group
			> This feature is provided to allow you to find and modify the prefabs that are loaded easily. 
		- Clear gizmo text
			> In the case that your gizmo text is showing after disabling a visualization feature - use this to clear the text from your scene world space.
	- Sink amount improvements
		- Adjust sink amounts more intuitivly
	- Rotation improvements
		- Some bug fixes here with some more intuitive UI and performance focus.
	- Documentation improvements
	- Artwork & spacing improvements.
	- Bug fixes 
	- New example scene set in space to demonstrate the new rotation features.

1.4.0
- Added interface start position property to allow users to move the active HMP UI in the scene view. 
- Improved interface UI.
- Added version number to the upper left of logo.

1.3.1
- Hotfix for Scale Min/Max growth problem.

1.3.0
- Added a physics-in-editor feature that allows you to allow physics to run inside the scene view.
	- Can only be used when HMPSystem > Animate Placement is disabled.
	- Pressing V (default) will simulate physics for the time you hold the button. 
	- Example of this feature can be found in the Town prefab - Feel free to play around with it using the gameobjects provided. Note that they can be used with prefabs also.
	- Also please note that this will effect ALL rigidbodies in the scene - whether they are placed using HMP, or not.
- When the Right Mouse button is held, the tool will no longer traverse the selection.
- Fixed issues with Example prefabs having default values of 0 as their Scale.
- Fixed issue with cached versions maintaining old invalid values.
- Fixed issue with Example prefab having missing images.
- Added new test cases around caching.
- Updated documentation.
- Improvements to code.
- Fixed a bug with Scale ranges not reacting to obscure scales.

1.2.0
- Small hotfix for an issue with versions failing from build.

1.1.0
- Huge update with tonnes of ease of use and quality of life improvements.
- New feature: Create prefab groups from folder structure.
	- You can now load all prefabs to help me place in just a couple of clicks.
	- This was added as adding row by row can be tedious if you have plenty of prefabs.
- New feature: Rotation Axis (Per prefab group)
	- Togglable by hotkey & new UI element added.
	- This feature allows you to change what axis the rotation system acts upon.
	- Change this value to 'All' for fully random rotations in all directions. Very useful for rocks or stones. 
- New feature: Scale range (Per prefab group)
	- You can now define a minimum and maximum scale range. Useful for Trees and Rocks.
- Event Hooks
	- You can now inject your own editor code by hooking to events from the EventHooks system. Just use HelpMePlace.Instance.Events to find a list of all events you can hook to.
- Large refactor
	- Improved performance and Editor usability.
- Minor UI Changes
- New example scene to demonstrate the Scale & Axis Rotation features, and Prefab Variant usage.
- New prefab examples added.
- Improved documentation and code commentry.

1.0.0:
- Initial Release
- Features included:
	General
	- System to place any prefab / gameobject easily and cleanly.
	- In-editor animated prefab placement. 
	- Automatic ground detection.
	- Custom sink amounts.
	- Strict placement or random modes.
	- Automatic parent organisation.
	- HDRP / URP Support.

	Customisable
	- Large amount of Rotation Modes.
	- Customisable Ground Layer. Place prefabs / gameobjects anywhere.
	- Multiple Ground Layer Support.

	User Experience
	- Map Projection helpers.
	- User friendly UI - Heavily customisable.
	- Almost everything driven by UI and Hotkey.
	- Random / Fixed / Selection driven, Quality of life focused UX.
	- Re-bindable keybinds.

	Code
	- Accessability for modification.
	- Namespaced to avoid any compatability.
	- Customisable and extendable.
	- Singleton instance.		
	- Extensive documentation & comments.