<!-- eslint-disable vue/multi-word-component-names -->
<script setup lang="ts">

withDefaults(defineProps<{
  isText?: boolean
  size?: '2xl' | 'md' | '3xs' | '2xs' | 'xs' | 'sm' | 'lg' | 'xl' | '3xl'
  logo: string
}>(), {
  isText: false,
  size: 'xl',
});
</script>

<template>
  <NuxtLink
    to="/"
    aria-label="Go back to home page"
  >
    <UUser
      :name=" isText ? 'Doc Convertor' : ''"
      :avatar="{
        src: logo,
        ui: { image: 'object-cover object-[center_25%]' }
      }"
      :size="size"
    />
  </NuxtLink>
</template>