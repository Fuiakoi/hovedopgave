using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI unityDeviceModelText;
	[SerializeField] TextMeshProUGUI unityCpuCoresText;
	[SerializeField] TextMeshProUGUI unityOsVersionText;
	[SerializeField] TextMeshProUGUI unityTotalMemoryText;
	[SerializeField] TextMeshProUGUI unityDeviceVendorText;

	[SerializeField] TextMeshProUGUI nativeDeviceModelText;
	[SerializeField] TextMeshProUGUI nativeCpuCoresText;
	[SerializeField] TextMeshProUGUI nativeOsVersionText;
	[SerializeField] TextMeshProUGUI nativeTotalMemoryText;
	[SerializeField] TextMeshProUGUI nativeDeviceVendorText;

	[SerializeField] Button deviceModelButton;
	[SerializeField] Button cpuCoresButton;
	[SerializeField] Button osVersionButton;
	[SerializeField] Button totalMemoryButton;
	[SerializeField] Button deviceVendorButton;

	IDeviceInfo _unityDeviceInfo;
	IDeviceInfo _nativeDeviceInfo;

	void Start()
	{
		ClearAllText();

		deviceModelButton.onClick.AddListener(ToggleDeviceModel);
		cpuCoresButton.onClick.AddListener(ToggleCpuCores);
		osVersionButton.onClick.AddListener(ToggleOsVersion);
		totalMemoryButton.onClick.AddListener(ToggleTotalMemory);
		deviceVendorButton.onClick.AddListener(ToggleDeviceVendor);

		_unityDeviceInfo = new EditorDeviceInfo();

#if UNITY_ANDROID && !UNITY_EDITOR
        _nativeDeviceInfo = new AndroidDeviceInfo();
#elif UNITY_IOS && !UNITY_EDITOR
        _nativeDeviceInfo = new AppleDeviceInfo();
#endif
	}

	void ClearAllText()
	{
		unityDeviceModelText.text = string.Empty;
		unityCpuCoresText.text = string.Empty;
		unityOsVersionText.text = string.Empty;
		unityTotalMemoryText.text = string.Empty;
		unityDeviceVendorText.text = string.Empty;

		nativeDeviceModelText.text = string.Empty;
		nativeCpuCoresText.text = string.Empty;
		nativeOsVersionText.text = string.Empty;
		nativeTotalMemoryText.text = string.Empty;
		nativeDeviceVendorText.text = string.Empty;
	}

	void ToggleDeviceModel()
	{
		if (string.IsNullOrEmpty(unityDeviceModelText.text))
		{
			// Unity
			var unityDeviceModel = _unityDeviceInfo.GetDeviceModel();
			unityDeviceModelText.text =
				string.Format(CultureInfo.InvariantCulture, "Device model: {0}", unityDeviceModel);

			// Native
			nativeDeviceModelText.text =
				GetNativeInfo(() => _nativeDeviceInfo.GetDeviceModel(), "Device model");
		}
		else
		{
			unityDeviceModelText.text = string.Empty;
			nativeDeviceModelText.text = string.Empty;
		}
	}

	void ToggleCpuCores()
	{
		if (string.IsNullOrEmpty(unityCpuCoresText.text))
		{
			// Unity
			var unityCpuCores = _unityDeviceInfo.GetCpuCores();
			unityCpuCoresText.text = string.Format(CultureInfo.InvariantCulture, "CPU cores: {0}", unityCpuCores);

			// Native
			nativeCpuCoresText.text =
				GetNativeInfo(() => _nativeDeviceInfo.GetCpuCores(), "CPU cores");
		}
		else
		{
			unityCpuCoresText.text = string.Empty;
			nativeCpuCoresText.text = string.Empty;
		}
	}

	void ToggleOsVersion()
	{
		if (string.IsNullOrEmpty(unityOsVersionText.text))
		{
			// Unity
			var unityOsVersion = _unityDeviceInfo.GetOsVersion();
			unityOsVersionText.text = string.Format(CultureInfo.InvariantCulture, "OS version: {0}", unityOsVersion);

			// Native
			nativeOsVersionText.text =
				GetNativeInfo(() => _nativeDeviceInfo.GetOsVersion(), "OS version");
		}
		else
		{
			unityOsVersionText.text = string.Empty;
			nativeOsVersionText.text = string.Empty;
		}
	}

	void ToggleTotalMemory()
	{
		if (string.IsNullOrEmpty(unityTotalMemoryText.text))
		{
			// Unity
			var unityTotalMemory = _unityDeviceInfo.GetDeviceMemory();
			unityTotalMemoryText.text =
				string.Format(CultureInfo.InvariantCulture, "Total memory (MB): {0}", unityTotalMemory);

			// Native
			nativeTotalMemoryText.text = GetNativeInfo(
				() => _nativeDeviceInfo.GetDeviceMemory(),
				"Total memory (MB)");
		}
		else
		{
			unityTotalMemoryText.text = string.Empty;
			nativeTotalMemoryText.text = string.Empty;
		}
	}

	void ToggleDeviceVendor()
	{
		if (string.IsNullOrEmpty(unityDeviceVendorText.text))
		{
			// Unity
			var vendor = _unityDeviceInfo.GetDeviceVendor();
			unityDeviceVendorText.text = string.Format(CultureInfo.InvariantCulture, "Device vendor: {0}", vendor);

			// Native
			nativeDeviceVendorText.text =
				GetNativeInfo(() => _nativeDeviceInfo.GetDeviceVendor(), "Device vendor");
		}
		else
		{
			unityDeviceVendorText.text = string.Empty;
			nativeDeviceVendorText.text = string.Empty;
		}
	}

	string GetNativeInfo(Func<string> getInfoFunc, string infoName)
	{
		if (_nativeDeviceInfo == null)
		{
			return "Native API not found";
		}

		var info = getInfoFunc();
		return string.IsNullOrEmpty(info) ? "Native API not found" : $"{infoName}: {info}";
	}

	string GetNativeInfo(Func<int> getInfoFunc, string infoName)
	{
		if (_nativeDeviceInfo == null)
		{
			return "Native API not found";
		}

		var info = getInfoFunc();
		return $"{infoName}: {info}";
	}

	string GetNativeInfo(Func<long> getInfoFunc, string infoName)
	{
		if (_nativeDeviceInfo == null)
		{
			return "Native API not found";
		}

		var info = getInfoFunc();
		return $"{infoName}: {info}";
	}
}