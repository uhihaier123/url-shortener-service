<script setup>
import { ref } from 'vue'
import axios from 'axios'

// Reactive state
const longUrl = ref('')
const shortUrl = ref('')
const isLoading = ref(false)
const error = ref('')
const urlHistory = ref([])
const urlStats = ref(null)


const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080'

// Methods
const handleSubmit = async () => {
  error.value = ''
  shortUrl.value = ''
  isLoading.value = true

  try {
    const response = await axios.post(`${API_BASE_URL}/api/urls/shorten`, {
      url: longUrl.value,
    })

    shortUrl.value = response.data.shortUrl
    urlStats.value = response.data
    urlHistory.value = [response.data, ...urlHistory.value].slice(0, 5)
  } catch (err) {
    if (err.response?.data?.errors) {
      const errorMessages = Object.values(err.response.data.errors).flat()
      error.value = errorMessages.join(', ')
    } else if (err.response?.data?.error) {
      error.value = err.response.data.error
    } else {
      error.value = 'Failed to shorten URL. Make sure the backend is running!'
    }
  } finally {
    isLoading.value = false
  }
}

const copyToClipboard = () => {
  navigator.clipboard.writeText(shortUrl.value)
  alert('Copied to clipboard!')
}

const handleClear = () => {
  longUrl.value = ''
  shortUrl.value = ''
  error.value = ''
  urlStats.value = null
}

const truncateUrl = (url, maxLength = 50) => {
  return url.length > maxLength ? url.substring(0, maxLength) + '...' : url
}

const formatDate = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

const refreshStats = async () => {
  if (!urlStats.value) return

  try {
    const response = await axios.get(`${API_BASE_URL}/api/urls/${urlStats.value.shortCode}/stats`)
    urlStats.value = response.data
    alert(`Updated! Click count: ${response.data.clickCount}`)
  } catch (err) {
    console.error('Failed to refresh stats:', err)
  }
}
</script>

<template>
  <div class="container">
    <h1>🔗 URL Shortener</h1>
    <p class="subtitle">Transform your long URLs into short, shareable links</p>

    <form @submit.prevent="handleSubmit" class="url-form">
      <input
        v-model="longUrl"
        type="text"
        placeholder="Enter your long URL here..."
        class="url-input"
        :disabled="isLoading"
      />

      <button type="submit" class="shorten-btn" :disabled="isLoading || !longUrl">
        <span v-if="isLoading" class="loading"></span>
        {{ isLoading ? 'Shortening...' : 'Shorten URL' }}
      </button>

      <button v-if="shortUrl || error" type="button" @click="handleClear" class="clear-btn">
        Clear & Start Over
      </button>
    </form>

    <div v-if="error" class="error-box">
      <span class="error-icon">⚠️</span>
      {{ error }}
    </div>

    <div v-if="shortUrl" class="result-box">
      <h3>✅ Your shortened URL:</h3>
      <div class="url-display">
        <a :href="shortUrl" target="_blank" rel="noopener noreferrer">
          {{ shortUrl }}
        </a>
        <button @click="copyToClipboard" class="copy-btn">📋 Copy</button>
      </div>

      <div v-if="urlStats" class="url-stats">
        <div class="stat-item">
          <span class="stat-label">📊 Clicks:</span>
          <span class="stat-value">{{ urlStats.clickCount }}</span>
        </div>
        <div class="stat-item">
          <span class="stat-label">📅 Created:</span>
          <span class="stat-value">{{ formatDate(urlStats.createdAt) }}</span>
        </div>
        <div class="stat-item">
          <span class="stat-label">🔗 Original:</span>
          <span class="stat-value truncate">{{ urlStats.originalUrl }}</span>
        </div>

        <button @click="refreshStats" class="refresh-btn">🔄 Refresh Stats</button>
      </div>
    </div>

    <div class="instructions">
      <h4>How to use:</h4>
      <ol>
        <li>Paste your long URL in the input field above</li>
        <li>Click "Shorten URL"</li>
        <li>Copy your shortened URL and share it!</li>
      </ol>
    </div>

    <div v-if="urlHistory.length > 0" class="history-section">
      <h4>📝 Recent URLs</h4>
      <div class="history-list">
        <div v-for="(item, index) in urlHistory" :key="index" class="history-item">
          <div class="history-original">
            {{ truncateUrl(item.originalUrl) }}
          </div>
          <div class="history-short">
            <a :href="item.shortUrl" target="_blank" rel="noopener noreferrer">
              {{ item.shortCode }}
            </a>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
