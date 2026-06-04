<template>
  <div class="animate-fade-up max-w-4xl mx-auto">
    <div class="mb-8">
      <h1 class="text-2xl font-heading font-bold text-main mb-1">
        Create New Event
      </h1>
      <p class="text-muted text-sm">
        Fill out the details below to publish your event to the marketplace.
      </p>
    </div>

    <form @submit.prevent="handleSubmit" class="space-y-8">
      <!-- Section 1: Basic Info -->
      <section class="glass-panel p-6 sm:p-8">
        <h2 class="text-lg font-heading font-semibold text-main mb-6 border-b border-border-main/50 pb-2">
          1. Basic Information
        </h2>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <BaseInput
            v-model="form.title"
            label="Event Title"
            placeholder="e.g. Summer Music Festival 2026"
            class="md:col-span-2"
            required
            data-testid="input-event-title"
          />

          <BaseSelect
            v-model="form.category"
            label="Category"
            :options="categoryOptions"
            required
            data-testid="select-event-category"
          />

          <BaseInput
            v-model="form.date"
            label="Date & Time"
            type="datetime-local"
            required
            data-testid="input-event-date"
          />

          <BaseInput
            v-model="form.location"
            label="Location / Venue"
            placeholder="e.g. MyDinh National Stadium"
            class="md:col-span-2"
            required
            data-testid="input-event-location"
          >
             <template #prefix>
                <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
             </template>
          </BaseInput>

          <!-- Mock Image Upload -->
          <div class="md:col-span-2 flex flex-col gap-1.5">
            <span class="text-sm font-medium text-muted pl-0.5">Event Banner (Mock Upload)</span>
            <div 
              class="relative border-2 border-dashed border-border-main rounded-xl p-8 text-center flex flex-col items-center justify-center transition-colors hover:border-primary/50 bg-bg-elevated/50 group"
              :class="{'border-primary/50 bg-primary/5': form.image}"
            >
              <input 
                type="file" 
                class="absolute inset-0 w-full h-full opacity-0 cursor-pointer" 
                accept="image/*"
                @change="handleImageUpload"
                data-testid="input-event-image"
              />
              <div v-if="form.image" class="text-primary flex flex-col items-center">
                <svg class="w-8 h-8 mb-2" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg>
                <span class="font-medium text-sm">{{ form.image.name }}</span>
              </div>
              <div v-else class="text-muted group-hover:text-main flex flex-col items-center transition-colors">
                <svg class="w-8 h-8 mb-2" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><circle cx="8.5" cy="8.5" r="1.5"></circle><polyline points="21 15 16 10 5 21"></polyline></svg>
                <span class="font-medium text-sm">Click to upload or drag and drop</span>
                <span class="text-xs text-dimmed mt-1">SVG, PNG, JPG or GIF (max. 5MB)</span>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Section 2: Dynamic Ticket Tiers -->
      <section class="glass-panel p-6 sm:p-8">
        <div class="flex items-center justify-between mb-6 border-b border-border-main/50 pb-2">
          <h2 class="text-lg font-heading font-semibold text-main">
            2. Ticket Tiers
          </h2>
          <span class="text-sm text-dimmed">{{ form.tiers.length }} Tiers</span>
        </div>

        <div class="space-y-4 mb-6">
          <TransitionGroup 
            enter-active-class="transition-all duration-300 ease-out"
            enter-from-class="opacity-0 -translate-x-4"
            enter-to-class="opacity-100 translate-x-0"
            leave-active-class="transition-all duration-200 ease-in absolute w-full"
            leave-from-class="opacity-100"
            leave-to-class="opacity-0 scale-95"
            move-class="transition-transform duration-300 ease-in-out"
          >
            <div 
              v-for="(tier, index) in form.tiers" 
              :key="tier.id"
              class="grid grid-cols-12 gap-4 items-start p-4 rounded-xl bg-surface/40 border border-border-light/20 relative group"
            >
              <div class="col-span-12 sm:col-span-5">
                <BaseInput
                  v-model="tier.name"
                  label="Tier Name"
                  placeholder="e.g. VIP, General Admission"
                  required
                  :data-testid="`input-tier-name-${index}`"
                />
              </div>
              <div class="col-span-6 sm:col-span-3">
                <BaseInput
                  v-model.number="tier.price"
                  type="number"
                  label="Price (₫)"
                  placeholder="0"
                  min="0"
                  required
                  :data-testid="`input-tier-price-${index}`"
                />
              </div>
              <div class="col-span-6 sm:col-span-3">
                <BaseInput
                  v-model.number="tier.quantity"
                  type="number"
                  label="Quantity"
                  placeholder="100"
                  min="1"
                  required
                  :data-testid="`input-tier-qty-${index}`"
                />
              </div>
              <div class="col-span-12 sm:col-span-1 flex items-center sm:justify-end sm:pt-7">
                <BaseButton
                  type="button"
                  variant="icon"
                  size="md"
                  class="text-danger/70 hover:text-danger hover:bg-danger/10 w-full sm:w-auto"
                  @click="removeTier(index)"
                  :disabled="form.tiers.length === 1"
                  :data-testid="`btn-remove-tier-${index}`"
                  aria-label="Remove Tier"
                >
                  <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
                </BaseButton>
              </div>
            </div>
          </TransitionGroup>
        </div>

        <BaseButton
          type="button"
          variant="outline"
          size="sm"
          class="w-full border-dashed"
          @click="addTier"
          data-testid="btn-add-tier"
        >
          <svg class="w-4 h-4 mr-1.5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
          Add Ticket Tier
        </BaseButton>
      </section>

      <!-- Submit Actions -->
      <div class="flex items-center justify-end gap-4 pt-4">
        <BaseButton
          type="button"
          variant="ghost"
          :disabled="isSubmitting"
          @click="$router.push('/organizer')"
        >
          Cancel
        </BaseButton>
        <BaseButton
          type="submit"
          variant="primary"
          size="lg"
          :is-loading="isSubmitting"
          data-testid="btn-submit-event"
        >
          Publish Event
        </BaseButton>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { delay } from '@/shared/composables/useMockApi'
import { useToast } from '@/shared/composables/useToast'
import BaseInput from '@/shared/components/BaseInput.vue'
import BaseSelect from '@/shared/components/BaseSelect.vue'
import BaseButton from '@/shared/components/BaseButton.vue'

const router = useRouter()
const toast = useToast()

const isSubmitting = ref(false)

// Options for the Category BaseSelect
const categoryOptions = [
  { label: 'Concert & Live Music', value: 'concerts' },
  { label: 'Sports & eSports', value: 'sports' },
  { label: 'Arts & Theatre', value: 'arts' },
  { label: 'Workshops & Education', value: 'workshops' },
  { label: 'Experiences', value: 'experiences' },
  { label: 'Other', value: 'others' },
]

// Generate unique IDs for tiers
let tierIdCounter = 0

const createEmptyTier = () => ({
  id: `tier_${tierIdCounter++}`,
  name: '',
  price: null,
  quantity: null,
})

// Reactive Form Data
const form = reactive({
  title: '',
  category: '',
  date: '',
  location: '',
  image: null,
  tiers: [createEmptyTier()]
})

// Handlers
const handleImageUpload = (event) => {
  const file = event.target.files[0]
  if (file) {
    form.image = file
  }
}

const addTier = () => {
  form.tiers.push(createEmptyTier())
}

const removeTier = (index) => {
  if (form.tiers.length > 1) {
    form.tiers.splice(index, 1)
  }
}

const handleSubmit = async () => {
  if (isSubmitting.value) return
  isSubmitting.value = true

  try {
    // Simulate network API save
    await delay(1500)
    
    toast.success('Event published successfully!', 4000)
    
    // Normally we'd clear form or redirect. Let's redirect to dashboard
    router.push('/organizer')
  } catch (error) {
    toast.error('Failed to publish event. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
/* Ensures the transition group wrapper doesn't collapse during absolute positioning leaves */
.space-y-4 {
  position: relative;
}
</style>
