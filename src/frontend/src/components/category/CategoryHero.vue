<template>
  <div 
    class="relative h-[240px] md:h-[280px] rounded-3xl overflow-hidden mb-8"
    :style="{ background: bgGradient }"
  >
    <!-- Content Overlay -->
    <div class="absolute inset-0 bg-gradient-to-r from-black/80 to-transparent flex items-center px-10 md:px-16">
      <div class="relative z-10 max-w-2xl">
        <div class="inline-flex items-center gap-2 mb-3 text-[13px] text-primary font-bold tracking-[0.08em] uppercase">
          {{ icon }} {{ title }} Sự kiện
        </div>
        <h1 class="font-heading text-4xl md:text-5xl font-bold text-white mb-3">
          {{ title }}
        </h1>
        <p class="text-white/70 text-base md:text-lg max-w-xl leading-relaxed">
          {{ subtitle }}
        </p>
      </div>

      <!-- Large Floating Icon -->
      <div class="absolute right-10 top-1/2 -translate-y-1/2 rotate-[-10deg] text-[140px] md:text-[180px] opacity-10 pointer-events-none select-none">
        {{ icon }}
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: String,
  subtitle: String,
  icon: String,
  bgGradient: String
})
</script>
