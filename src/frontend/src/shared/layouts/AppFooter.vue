<template>
  <footer class="border-t border-border-main/50 mt-auto">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-10">
        <!-- Brand Column -->
        <div class="md:col-span-1">
          <router-link to="/" class="flex items-center gap-2.5 mb-4 group">
            <div
              class="w-8 h-8 rounded-lg bg-primary/15 border border-primary/25 flex items-center justify-center
                     transition-all duration-300 group-hover:bg-primary/20"
            >
              <svg class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                <path d="M2 9a3 3 0 0 1 0 6v2a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-2a3 3 0 0 1 0-6V7a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2Z" />
                <path d="M13 5v2" /><path d="M13 17v2" /><path d="M13 11v2" />
              </svg>
            </div>
            <span class="font-heading font-bold text-main tracking-tight">
              Ticket<span class="text-primary">Hub</span>
            </span>
          </router-link>
          <p class="text-sm text-muted leading-relaxed">
            Vietnam's premier platform for discovering and booking events, concerts, and experiences.
          </p>
        </div>

        <!-- Link Columns -->
        <div v-for="group in footerGroups" :key="group.title">
          <h4 class="text-xs font-semibold text-dimmed uppercase tracking-wider mb-4">
            {{ group.title }}
          </h4>
          <ul class="space-y-2.5">
            <li v-for="link in group.links" :key="link.label">
              <router-link
                :to="link.to"
                class="text-sm text-muted hover:text-main transition-colors duration-200"
              >
                {{ link.label }}
              </router-link>
            </li>
          </ul>
        </div>
      </div>

      <!-- Bottom Bar -->
      <div class="flex flex-col sm:flex-row items-center justify-between gap-4 mt-12 pt-8 border-t border-border-main/40">
        <p class="text-xs text-dimmed">
          &copy; {{ currentYear }} TicketHub. All rights reserved.
        </p>
        <div class="flex items-center gap-5">
          <a
            v-for="social in socials"
            :key="social.label"
            :href="social.href"
            target="_blank"
            rel="noopener noreferrer"
            :aria-label="social.label"
            class="text-dimmed hover:text-primary transition-colors duration-200"
          >
            <component :is="social.icon" class="w-4 h-4" />
          </a>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
/**
 * AppFooter — Public site footer.
 *
 * Contains brand info, navigation link groups, social icons,
 * and copyright bar. Follows the dark glassmorphism theme.
 */
import { h } from 'vue'

const currentYear = new Date().getFullYear()

const footerGroups = [
  {
    title: 'Explore',
    links: [
      { label: 'Concerts', to: '/' },
      { label: 'Sports', to: '/' },
      { label: 'Arts & Theatre', to: '/' },
      { label: 'Workshops', to: '/' },
    ],
  },
  {
    title: 'For Organizers',
    links: [
      { label: 'Dashboard', to: '/organizer' },
      { label: 'Create Event', to: '/organizer/create' },
      { label: 'Pricing', to: '/' },
    ],
  },
  {
    title: 'Support',
    links: [
      { label: 'Help Center', to: '/' },
      { label: 'Privacy Policy', to: '/' },
      { label: 'Terms of Service', to: '/' },
    ],
  },
]

/* Inline SVG icon render functions — avoids external icon deps */
const IconGithub = (_, { attrs }) =>
  h('svg', { viewBox: '0 0 24 24', fill: 'currentColor', ...attrs }, [
    h('path', { d: 'M12 2C6.477 2 2 6.477 2 12c0 4.42 2.865 8.166 6.839 9.489.5.092.682-.217.682-.482 0-.237-.009-.866-.013-1.7-2.782.604-3.369-1.34-3.369-1.34-.454-1.156-1.11-1.463-1.11-1.463-.908-.62.069-.608.069-.608 1.003.07 1.531 1.03 1.531 1.03.892 1.529 2.341 1.088 2.91.832.092-.647.35-1.088.636-1.338-2.22-.253-4.555-1.11-4.555-4.943 0-1.091.39-1.984 1.029-2.683-.103-.253-.446-1.27.098-2.647 0 0 .84-.269 2.75 1.025A9.564 9.564 0 0112 6.844a9.59 9.59 0 012.504.337c1.909-1.294 2.747-1.025 2.747-1.025.546 1.377.203 2.394.1 2.647.64.699 1.028 1.592 1.028 2.683 0 3.842-2.339 4.687-4.566 4.935.359.309.678.919.678 1.852 0 1.336-.012 2.415-.012 2.743 0 .267.18.578.688.48C19.138 20.161 22 16.416 22 12c0-5.523-4.477-10-10-10z' }),
  ])

const IconTwitter = (_, { attrs }) =>
  h('svg', { viewBox: '0 0 24 24', fill: 'currentColor', ...attrs }, [
    h('path', { d: 'M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z' }),
  ])

const IconMail = (_, { attrs }) =>
  h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round', ...attrs }, [
    h('rect', { width: '20', height: '16', x: '2', y: '4', rx: '2' }),
    h('path', { d: 'm22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7' }),
  ])

const socials = [
  { label: 'GitHub', href: '#', icon: IconGithub },
  { label: 'Twitter', href: '#', icon: IconTwitter },
  { label: 'Email', href: 'mailto:hello@tickethub.vn', icon: IconMail },
]
</script>
