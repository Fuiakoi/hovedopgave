using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
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

	[SerializeField] TextMeshProUGUI countdownText;

	[SerializeField] Button deviceModelButton;
	[SerializeField] Button cpuCoresButton;
	[SerializeField] Button osVersionButton;
	[SerializeField] Button totalMemoryButton;
	[SerializeField] Button deviceVendorButton;

	[SerializeField] Button clearButton;

	IDeviceInfo _unityDeviceInfo;
	IDeviceInfo _nativeDeviceInfo;

	CancellationTokenSource _tokenDeviceModel;
	CancellationTokenSource _tokenCpuCores;
	CancellationTokenSource _tokenOsVersion;
	CancellationTokenSource _tokenDeviceMemory;
	CancellationTokenSource _tokenDeviceVendor;
	CancellationTokenSource _tokenClearText;

	bool _wasPressed;

	void Start()
	{
		_tokenClearText = new CancellationTokenSource();
		
		ClearAllText();

		deviceModelButton.onClick.AddListener(ToggleDeviceModel);
		cpuCoresButton.onClick.AddListener(ToggleCpuCores);
		osVersionButton.onClick.AddListener(ToggleOsVersion);
		totalMemoryButton.onClick.AddListener(ToggleTotalMemory);
		deviceVendorButton.onClick.AddListener(ToggleDeviceVendor);

		clearButton.onClick.AddListener(ClearAfterDelay);

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

		countdownText.text = string.Empty;
	}

	async void ToggleDeviceModel()
	{
		_tokenDeviceModel?.Cancel();
		_tokenDeviceModel = new CancellationTokenSource();
		if (string.IsNullOrEmpty(unityDeviceModelText.text))
		{
			// Unity
			var unityDeviceModel = await _unityDeviceInfo.GetDeviceModel(_tokenDeviceModel.Token);
			unityDeviceModelText.text =
				string.Format(CultureInfo.InvariantCulture, "Device model: {0}", unityDeviceModel);

			// Native
			nativeDeviceModelText.text =
				await GetNativeInfo(() => _nativeDeviceInfo.GetDeviceModel(_tokenDeviceModel.Token), "Device model");
		}
		else
		{
			unityDeviceModelText.text = string.Empty;
			nativeDeviceModelText.text = string.Empty;
		}
	}

	async void ToggleCpuCores()
	{
		_tokenCpuCores?.Cancel();
		_tokenCpuCores = new CancellationTokenSource();
		if (string.IsNullOrEmpty(unityCpuCoresText.text))
		{
			// Unity
			var unityCpuCores = await _unityDeviceInfo.GetCpuCores(_tokenCpuCores.Token);
			unityCpuCoresText.text = string.Format(CultureInfo.InvariantCulture, "CPU cores: {0}", unityCpuCores);

			// Native
			nativeCpuCoresText.text =
				await GetNativeInfo(() => _nativeDeviceInfo.GetCpuCores(_tokenCpuCores.Token), "CPU cores");
		}
		else
		{
			unityCpuCoresText.text = string.Empty;
			nativeCpuCoresText.text = string.Empty;
		}
	}

	async void ToggleOsVersion()
	{
		_tokenOsVersion?.Cancel();
		_tokenOsVersion = new CancellationTokenSource();
		if (string.IsNullOrEmpty(unityOsVersionText.text))
		{
			// Unity
			var unityOsVersion = await _unityDeviceInfo.GetOsVersion(_tokenOsVersion.Token);
			unityOsVersionText.text = string.Format(CultureInfo.InvariantCulture, "OS version: {0}", unityOsVersion);

			// Native
			nativeOsVersionText.text =
				await GetNativeInfo(() => _nativeDeviceInfo.GetOsVersion(_tokenOsVersion.Token), "OS version");
		}
		else
		{
			unityOsVersionText.text = string.Empty;
			nativeOsVersionText.text = string.Empty;
		}
	}

	async void ToggleTotalMemory()
	{
		_tokenDeviceMemory?.Cancel();
		_tokenDeviceMemory = new CancellationTokenSource();
		if (string.IsNullOrEmpty(unityTotalMemoryText.text))
		{
			// Unity
			var unityTotalMemory = await _unityDeviceInfo.GetDeviceMemory(_tokenDeviceMemory.Token);
			unityTotalMemoryText.text =
				string.Format(CultureInfo.InvariantCulture, "Total memory (MB): {0}", unityTotalMemory);

			// Native
			nativeTotalMemoryText.text = await GetNativeInfo(
				() => _nativeDeviceInfo.GetDeviceMemory(_tokenDeviceMemory.Token),
				"Total memory (MB)");
		}
		else
		{
			unityTotalMemoryText.text = string.Empty;
			nativeTotalMemoryText.text = string.Empty;
		}
	}

	async void ToggleDeviceVendor()
	{
		_tokenDeviceVendor?.Cancel();
		_tokenDeviceVendor = new CancellationTokenSource();
		if (string.IsNullOrEmpty(unityDeviceVendorText.text))
		{
			// Unity
			var vendor = await _unityDeviceInfo.GetDeviceVendor(_tokenDeviceVendor.Token);
			unityDeviceVendorText.text = string.Format(CultureInfo.InvariantCulture, "Device vendor: {0}", vendor);

			// Native
			nativeDeviceVendorText.text =
				await GetNativeInfo(() => _nativeDeviceInfo.GetDeviceVendor(_tokenDeviceVendor.Token), "Device vendor");
		}
		else
		{
			unityDeviceVendorText.text = string.Empty;
			nativeDeviceVendorText.text = string.Empty;
		}
	}

	async Task<string> GetNativeInfo(Func<Task<string>> getInfoFunc, string infoName)
	{
		if (_nativeDeviceInfo == null)
		{
			return "Native API not found";
		}

		var info = await getInfoFunc();
		return string.IsNullOrEmpty(info) ? "Native API not found" : $"{infoName}: {info}";
	}

	async Task<string> GetNativeInfo(Func<Task<int>> getInfoFunc, string infoName)
	{
		if (_nativeDeviceInfo == null)
		{
			return "Native API not found";
		}

		var info = await getInfoFunc();
		return $"{infoName}: {info}";
	}

	async Task<string> GetNativeInfo(Func<Task<long>> getInfoFunc, string infoName)
	{
		if (_nativeDeviceInfo == null)
		{
			return "Native API not found";
		}

		var info = await getInfoFunc();
		return $"{infoName}: {info}";
	}

	IEnumerator CountdownTimer()
	{
		int counter = 3;

		while (counter > 0)
		{
			countdownText.text = counter.ToString();
			yield return new WaitForSeconds(1);
			counter--;
		}

		countdownText.text = string.Empty;
	}

	async void ClearAfterDelay()
	{
		if (_wasPressed)
		{
			_tokenClearText?.Cancel();
			_wasPressed = false;
			return;
		}

		_wasPressed = true;

		StartCoroutine(CountdownTimer());

		try
		{
			await Task.Delay(3000, _tokenClearText.Token);
		}
		catch (TaskCanceledException)
		{
			_tokenClearText = new CancellationTokenSource();
			StopAllCoroutines();
			countdownText.text = string.Empty;
			return;
		}

		_wasPressed = false;
		ClearAllText();
	}
}