using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent (typeof (ARTrackedImageManager))]
public class PlaceTrackedImage : MonoBehaviour
{
    // Reference to AR tracked image manager component  
    private ARTrackedImageManager _trackedImagesManager;

   public GameObject[] ArPrefabs;
   private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();
    void Awake()
    {
        _trackedImagesManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable() {
        _trackedImagesManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable() {
        _trackedImagesManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs) {
        foreach (var trackedImage in eventArgs.added) {
            var imageName = trackedImage.referenceImage.name;
            // Now loop over the array of prefabs
            foreach (var curPrefab in ArPrefabs) {
                //Check whether this prefab matches the tracked image name, and that
                // the prefab hasn't already been created
                if (string.Compare(curPrefab.name, imageName, System.StringComparison.OrdinalIgnoreCase) == 0 && !_instantiatedPrefabs.ContainsKey(imageName)) {
                    // Instantiate the prefab, parenting it to the ARTrackedImage
                    var newPrefab = Instantiate (curPrefab, trackedImage.transform);
                    // Add the created prefab to our array
                    _instantiatedPrefabs[imageName] = newPrefab;
                }
            }
        }

        // For all prefabs that have been created so far, set them active or not depending / on whether their corresponding image is currently being tracked
        foreach (var trackedImage in eventArgs.updated) {
            _instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }

        // If the AR subsystem has given up looking for a tracked image
        foreach (var trackedImage in eventArgs.removed) {
            //Destroy prefab
            Destroy(_instantiatedPrefabs [trackedImage.referenceImage.name]);
            _instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
        }
    }
}
